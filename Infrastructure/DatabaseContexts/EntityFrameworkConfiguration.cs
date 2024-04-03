using Domain.BaseTypes;
using Domain.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.DatabaseContexts;

public static class EntityFrameworkConfiguration
{
    // TODO: Replace List<> properties in data model with ICollection<>
    // TODO: Configure research lazy-loading and configure loading behaviour

    public static void Configure(ModelBuilder modelBuilder)
    {
        Configure(modelBuilder.Entity<User>());
        Configure(modelBuilder.Entity<Administrator>());
        Configure(modelBuilder.Entity<Student>());

        Configure(modelBuilder.Entity<Comment>());
        Configure(modelBuilder.Entity<Event>());
        Configure(modelBuilder.Entity<Post>());

        Configure(modelBuilder.Entity<Report>());
        Configure(modelBuilder.Entity<CommentReport>());
        Configure(modelBuilder.Entity<EventReport>());
        Configure(modelBuilder.Entity<PostReport>());

        using var conventionBlock = modelBuilder.Model.DelayConventions();

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (entity.ClrType.IsSubclassOf(typeof(BaseEntity))
                    && !entity.ClrType.IsSubclassOf(typeof(User))
                    && !entity.ClrType.IsSubclassOf(typeof(Report)))
                {
                    /* Magic below comes from https://stackoverflow.com/questions/75622471/entity-framework-core-how-to-specify-the-same-set-of-primary-keys-for-all-entit
                     * It configures Id as the primary key and Guid as an alternative key
                     * for all types derived from BaseEntity (which is all types in our data model)
                     */
                    if (property.Name == nameof(BaseEntity.Id))
                    {
                        entity.SetPrimaryKey(property);
                        property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                    else if (property.Name == nameof(BaseEntity.Guid))
                    {
                        entity.AddKey(property);
                        property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                }
                else if (property.ClrType.IsEnum)
                {
                    /* Magic below comes from https://stackoverflow.com/questions/47721246/ef-core-2-0-enums-stored-as-string
                     * It configures all enums in our data model to be stored as strings in database
                     * Which meands that we are allowed to change their values order of definition
                     * BUT WE CANNOT CHANGE ENUM VALUES NAMES!!!
                     * ... unless someone fixes the ef migration manually
                     */
                    var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
                    var converterInstance = (ValueConverter)Activator.CreateInstance(converterType)!;
                    property.SetValueConverter(converterInstance);
                    property.SetIsUnicode(false);
                    property.SetMaxLength(32);
                }
            }
        }
    }

    private static void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.HasBaseType<User>();
    }

    private static void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder
            .HasOne(x => x.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.Post)
            .WithMany(x => x.Comments)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Likers)
            .WithMany();

        /* This one is important!
         * 
         * By default when a Comment would be deleted, its responses would stop being resposes
         * and instead would become direct Comments on the parent Post.
         * Here we configure the OnDelete behaviour to also delete responses.
         */
        builder
            .HasOne(x => x.InResponseTo)
            .WithMany(x => x.Responses)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .HasOne(x => x.Organizer)
            .WithMany(x => x.OrganizedEvents)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(x => x.Interested)
            .WithMany(x => x.SubscribedEvents);

        builder
            .HasMany(x => x.Participants)
            .WithMany();

        // Relationship with Post is configured in Post
    }

    private static void Configure(EntityTypeBuilder<Post> builder)
    {
        builder
            .HasOne(x => x.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.Event)
            .WithMany(x => x.Posts)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with Comment is configured in Comment
    }

    private static void Configure(EntityTypeBuilder<Report> builder)
    {
        /* TPH Mapping Strategy stands for Table per Hierarchy
         * 
         * It means instances of every Report subtype will be placed in the same table.
         * Since they are almost identical (from EF point of view they are exactly identical),
         * there are no downsides to this approach.
         * 
         * Docs here: https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
         */
        builder.UseTphMappingStrategy();

        builder
            .HasOne(x => x.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.Responder)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void Configure(EntityTypeBuilder<CommentReport> builder)
    {
        builder.HasBaseType<Report>();

        builder
            .HasOne(x => x.ReportedComment)
            .WithMany()
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(EntityTypeBuilder<EventReport> builder)
    {
        builder.HasBaseType<Report>();

        builder
            .HasOne(x => x.ReportedEvent)
            .WithMany()
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(EntityTypeBuilder<PostReport> builder)
    {
        builder.HasBaseType<Report>();

        builder
            .HasOne(x => x.ReportedPost)
            .WithMany()
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasBaseType<User>();

        builder
            .HasMany(x => x.Friends)
            .WithMany();

        // All relationship with Event, Post and Comment are configured in their respective classes
    }

    private static void Configure(EntityTypeBuilder<User> builder)
    {
        /* See Configure(Report) above for detailed explanation what line below does
         * 
         * Despite Student class being seemingly more complex then Administator
         * in reality it has only a few additional columns - datetime, text and 2 booleans,
         * so the performance gain from having everything in the same table is still worth much more
         * then the fact that for Administrator these columns will be null.
         * Especially since there will be much more normal users than admins using this app.
         */
        builder.UseTphMappingStrategy();

        builder
            .Property(x => x.Username)
            .HasMaxLength(64);
        builder
            .Property(x => x.Email)
            .HasMaxLength(64);

        builder
            .Property(x => x.SaltedPasswordHash)
            .HasMaxLength(64)
            .IsFixedLength(true);
    }
}

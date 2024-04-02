using Domain.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.DatabaseContexts;

public static class EntityFrameworkConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        Configure(modelBuilder.Entity<User>());

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (!property.ClrType.IsEnum)
                    continue;

                // Magic from stackoverflow (EF Core 2.0 Enums stored as strings [duplicate])
                // Makes all enums in our data model to be stored as strings in database
                // Meaning that we are allowed to change their order of definition
                // BUT NOT THEIR NAMES!!!
                var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
                var converterInstance = (ValueConverter)Activator.CreateInstance(converterType, new ConverterMappingHints())!;
                property.SetValueConverter(converterInstance);
            }
        }

    }

    private static void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.HasBaseType<User>();
    }

    private static void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Guid);

        builder.HasOne(x => x.Author)
            .WithMany();

        builder.HasOne(x => x.Post)
            .WithMany(x => x.Comments);

        builder.HasMany(x => x.Likers)
            .WithMany();

        builder.HasOne(x => x.InResponseTo)
            .WithMany(x => x.Responses);
    }

    private static void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Guid);

        builder.HasOne(x => x.Organizer)
            .WithMany(x => x.OrganizedEvents);

        builder.HasMany(x => x.Interested)
            .WithMany(x => x.SubscribedEvents);

        builder.HasMany(x => x.Participants)
            .WithMany();

        // Relationship with Post is configured in Post
    }

    private static void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Guid);

        builder.HasOne(x => x.Author)
            .WithMany();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Posts);

        // Relationship with Comment is configured in Comment
    }

    private static void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Guid);

        builder.HasOne(x => x.Author)
            .WithMany();

        builder.HasOne(x => x.Responder)
            .WithMany();
    }

    private static void Configure(EntityTypeBuilder<EventReport> builder)
    {
        builder.HasBaseType<Report>();

        // TODO: Shared property - target
    }

    // TODO: Other report types

    private static void Configure(EntityTypeBuilder<Student> builder)
    {
        // TODO: Finish
    }

    private static void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => x.Guid);

        builder.Property(x => x.Username)
            .HasMaxLength(64);
        builder.Property(x => x.Email)
            .HasMaxLength(64);

        builder.Property(x => x.SaltedPasswordHash)
            .HasMaxLength(64);
    }
}

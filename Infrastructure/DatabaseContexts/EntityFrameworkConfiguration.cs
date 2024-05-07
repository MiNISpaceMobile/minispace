using Domain.BaseTypes;
using Domain.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DatabaseContexts;

public static class EntityFrameworkConfiguration
{
    /* TODO: Add custom wrapper around IQueryable
     * that exposes Include and ThenInclude methods
     * and return it from IRepository's Get and GetAll methods
     */

    public static IServiceCollection AddEFContext<DbContextType>(this IServiceCollection services) where DbContextType : DbContext
        => services.AddDbContext<DbContext, DbContextType>();

    /* Lazy-loading may introduce some performance issues
     * and we may want to think about it in the future.
     * 
     * For more details see https://www.reddit.com/r/csharp/comments/wxjsd7/how_do_you_manage_nonloaded_navigation_properties/
     */
    public static void Configure(this DbContextOptionsBuilder options)
        => options.UseLazyLoadingProxies(true)
                  .ConfigureWarnings(warnings => warnings.Log(CoreEventId.NavigationLazyLoading));

    public static void Configure(this ModelBuilder model)
    {
        model.Entity<User>().Configure();
        model.Entity<Administrator>().Configure();
        model.Entity<Student>().Configure();

        model.Entity<Event>().Configure();
        model.Entity<Feedback>().Configure();
        model.Entity<Post>().Configure();
        model.Entity<Comment>().Configure();

        model.Entity<Report>().Configure();
        model.Entity<EventReport>().Configure();
        model.Entity<PostReport>().Configure();
        model.Entity<CommentReport>().Configure();

        model.Entity<BaseNotification>().Configure();
        model.Entity<Notification>().Configure();
        model.Entity<SocialNotification>().Configure();
        model.Entity<FriendRequest>().Configure();

        using var conventionBlock = model.Model.DelayConventions();

        foreach (var entity in model.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (entity.ClrType.IsSubclassOf(typeof(BaseEntity)) &&
                    property.Name == nameof(BaseEntity.Guid) &&
                    property.ClrType == typeof(Guid))
                {
                    property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
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

    #region Users
    private static void Configure(this EntityTypeBuilder<User> type)
    {
        type.HasKey(x => x.Guid);

        /* See Configure(Report) above for detailed explanation what line below does
         * 
         * Despite Student class being seemingly more complex then Administator
         * in reality it has only a few additional columns - datetime, text and 2 booleans,
         * so the performance gain from having everything in the same table is still worth much more
         * then the fact that for Administrator these columns will be null.
         * Especially since there will be much more normal users than admins using this app.
         */
        type.UseTphMappingStrategy();

        type.Property(x => x.FirstName)
            .HasMaxLength(32);

        type.Property(x => x.LastName)
            .HasMaxLength(64);

        type.Property(x => x.Email)
            .HasMaxLength(128);

        //type.Property(x => x.SaltedPasswordHash)
        //    .HasMaxLength(64)
        //    .IsFixedLength(true);
    }

    private static void Configure(this EntityTypeBuilder<Administrator> type)
    {
        type.HasBaseType<User>();
    }

    private static void Configure(this EntityTypeBuilder<Student> type)
    {
        type.HasBaseType<User>();

        type.HasMany(x => x.Friends)
            .WithMany();

        // This property simply concatenates results of other properties
        type.Ignore(x => x.AllNotifications);

        // All relationships with Event, Feedback, Post and Comment are configured in their respective classes
    }
    #endregion Users

    #region Events/Feedback/Posts/Comments
    private static void Configure(this EntityTypeBuilder<Event> type)
    {
        type.HasKey(x => x.Guid);

        type.HasOne(x => x.Organizer)
            .WithMany(x => x.OrganizedEvents)
            .HasForeignKey(x => x.OrganizerId)
            .OnDelete(DeleteBehavior.SetNull);

        type.HasMany(x => x.Interested)
            .WithMany(x => x.SubscribedEvents);

        type.HasMany(x => x.Participants)
            .WithMany(x => x.JoinedEvents);

        // All relationships with Feedback and Post are configured in their respective classes
    }

    private static void Configure(this EntityTypeBuilder<Feedback> type)
    {
        type.HasKey(x => new { x.AuthorId, x.EventId });

        type.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        type.HasOne(x => x.Event)
            .WithMany(x => x.Feedback)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(this EntityTypeBuilder<Post> type)
    {
        type.HasKey(x => x.Guid);

        type.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        type.HasOne(x => x.Event)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with Comment is configured in Comment
    }

    private static void Configure(this EntityTypeBuilder<Comment> type)
    {
        type.HasKey(x => x.Guid);

        type.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        type.HasOne(x => x.Post)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        type.HasMany(x => x.Likers)
            .WithMany();

        /* This one is important!
         * 
         * By default when a Comment would be deleted, its responses would stop being resposes
         * and instead would become direct Comments on the parent Post.
         * Here we configure the OnDelete behaviour to also delete responses.
         */
        type.HasOne(x => x.InResponseTo)
            .WithMany(x => x.Responses)
            .HasForeignKey(x => x.InResponeseToId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    #endregion Events/Feedback/Posts/Comments

    #region Reports
    private static void Configure(this EntityTypeBuilder<Report> type)
    {
        type.HasKey(x => x.Guid);

        /* TPH Mapping Strategy stands for Table per Hierarchy
         * 
         * It means instances of every Report subtype will be placed in the same table.
         * Since they are almost identical (from EF point of view they are exactly identical),
         * there are no downsides to this approach.
         * 
         * Docs here: https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
         */
        type.UseTphMappingStrategy();

        type.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        type.HasOne(x => x.Responder)
            .WithMany()
            .HasForeignKey(x => x.ResponderId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private static void Configure(this EntityTypeBuilder<EventReport> type)
    {
        type.HasBaseType<Report>();

        type.HasOne(x => x.ReportedEvent)
            .WithMany()
            .HasForeignKey(x => x.ReportedEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(this EntityTypeBuilder<PostReport> type)
    {
        type.HasBaseType<Report>();

        type.HasOne(x => x.ReportedPost)
            .WithMany()
            .HasForeignKey(x => x.ReportedPostId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void Configure(this EntityTypeBuilder<CommentReport> type)
    {
        type.HasBaseType<Report>();

        type.HasOne(x => x.ReportedComment)
            .WithMany()
            .HasForeignKey(x => x.ReportedCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    #endregion Reposts

    #region Notifications
    public static void Configure(this EntityTypeBuilder<BaseNotification> type)
    {
        type.HasKey(x => x.Guid);

        /* The following code is incompatible with navigations configured in BaseNotification subclasses
         * But it reflects the idea, which AllNotifications property serves
         */
        //type.HasOne(x => x.Target)
        //    .WithMany(x => x.AllNotifications)
        //    .HasForeignKey(x => x.TargetId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }

    public static void Configure(this EntityTypeBuilder<Notification> type)
    {
        type.HasBaseType<BaseNotification>();

        type.HasOne(x => x.Target)
            .WithMany(x => x.PersonalNotifications)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public static void Configure(this EntityTypeBuilder<SocialNotification> type)
    {
        type.HasBaseType<BaseNotification>();

        type.HasOne(x => x.Target)
            .WithMany(x => x.SocialNotifications)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Cascade);

        type.HasOne(x => x.Friend)
            .WithMany()
            .HasForeignKey(x => x.FriendId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public static void Configure(this EntityTypeBuilder<FriendRequest> type)
    {
        type.HasBaseType<BaseNotification>();

        type.HasOne(x => x.Target)
            .WithMany(x => x.ReceivedFriendRequests)
            .HasForeignKey(x => x.TargetId)
            .OnDelete(DeleteBehavior.Cascade);

        type.HasOne(x => x.Author)
            .WithMany(x => x.SentFriendRequests)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    #endregion Notifications
}

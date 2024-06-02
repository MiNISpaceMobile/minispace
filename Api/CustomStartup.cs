using Domain.Abstractions;
using Domain.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class CustomStartup
{
    public static void PerformCustomStartupActions(this WebApplication app, bool resetDb, bool generateDevUsersJwt)
    {
        if (app.Environment.IsDevelopment() && resetDb)
            app.DeleteDevelopmentDatabase();
        app.CreateOrUpdateDevelopmentDatabase();
        app.SeedDatabaseIfEmpty();
        if (generateDevUsersJwt)
            app.GenerateDevelopmentAdminJwt();
    }

    private static void DeleteDevelopmentDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        dbContext.Database.EnsureDeleted();
        app.Logger.LogWarning("Database was deleted in order to be reconstructed from scratch");
    }

    private static void CreateOrUpdateDevelopmentDatabase(this WebApplication app)
    {
        /* Lines of code below automatically create and/or update development database.
         * You can find it in a subfolder 'MinispaceDb' of app's working directory,
         * which defaults to Api project directory. If something (like migrations) breaks horribly,
         * you can always recreate from scratch it - just delete it
         * or if you want it to always be recreated from scratch
         * pass 'resetDb: true' to 'PerformCustomStartupActions' function in 'Program.cs'.
         */
        using var scope = app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        dbContext.Database.Migrate();
        app.Logger.LogInformation("Ensured database is created and up-to-date with migrations");
    }

    private static void SeedDatabaseIfEmpty(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        if (uow.Repository<Event>().GetAll().Any() ||
            uow.Repository<User>().GetAll().Any())
        {
            app.Logger.LogInformation("Skipped database seeding since it was not empty");
            return;
        }

        var epoch = DateTime.UnixEpoch;
        var today = DateTime.UtcNow.Date.AddHours(9);
        var week = new DateTime[7]
        {
            today.AddDays(-2),
            today.AddDays(-1),
            today,
            today.AddDays(+1),
            today.AddDays(+2),
            today.AddDays(+3),
            today.AddDays(+4),
        };

        #region Users

        var ad = new User("Dev", "Admin", "dev.admin@pw.edu.pl", epoch, "DevAdmin")
        { Guid = Guid.Parse("10000000-0000-0000-0000-000000000000"), IsAdmin = true, IsOrganizer = true };

        var or = new User("Dev", "Organizer", "dev.organizer@pw.edu.pl", epoch, "DevOrganizer")
        { Guid = Guid.Parse("20000000-0000-0000-0000-000000000000"), IsOrganizer = true };

        var st = new User("Dev", "Student", "dev.student@pw.edu.pl", epoch, "DevStudent")
        { Guid = Guid.Parse("30000000-0000-0000-0000-000000000000") };

        ad.Friends.Add(or);
        or.Friends.Add(ad);

        var o2s = new FriendRequest(st, or)
        { Guid = Guid.Parse("00000001-0000-0000-0000-000000000000") };
        or.SentFriendRequests.Add(o2s);
        st.ReceivedFriendRequests.Add(o2s);

        var s2a = new FriendRequest(ad, st)
        { Guid = Guid.Parse("00000002-0000-0000-0000-000000000000") };
        st.SentFriendRequests.Add(s2a);
        ad.ReceivedFriendRequests.Add(s2a);

        uow.Repository<User>().AddMany([ad, or, st]);
        uow.Repository<FriendRequest>().AddMany([o2s, s2a]);

        #endregion Users

        #region Events

        var ev1 = new Event(or, "Ev1", "Des1", EventCategory.Uncategorized, week[0], week[1], week[2], "Loc1", null, null)
        { Guid = Guid.Parse("01000000-0000-0000-0000-000000000000")};

        var ev2 = new Event(or, "Ev2", "Des2", EventCategory.Uncategorized, week[1], week[2], week[4], "Loc2", 10, null)
        { Guid = Guid.Parse("02000000-0000-0000-0000-000000000000")};
        
        var ev3 = new Event(or, "Ev3", "Des3", EventCategory.Uncategorized, week[2], week[4], week[6], "Loc3", null, 10)
        { Guid = Guid.Parse("03000000-0000-0000-0000-000000000000")};
        
        var ev4 = new Event(or, "Ev4", "Des4", EventCategory.Uncategorized, week[4], week[5], week[6], "Loc4", 10, 10)
        { Guid = Guid.Parse("04000000-0000-0000-0000-000000000000")};

        or.OrganizedEvents.Add(ev1);
        or.OrganizedEvents.Add(ev2);
        or.OrganizedEvents.Add(ev3);
        or.OrganizedEvents.Add(ev4);

        ev1.Participants.Add(st);
        st.JoinedEvents.Add(ev1);
        ev1.Feedback.Add(new Feedback(st, ev1, 4));

        ev2.Interested.Add(ad);
        ad.SubscribedEvents.Add(ev2);

        ev3.Interested.Add(st);
        st.SubscribedEvents.Add(ev3);
        ev3.Participants.Add(ad);
        ad.JoinedEvents.Add(ev3);

        uow.Repository<Event>().AddMany([ev1, ev2, ev3, ev4]);

        #endregion Events

        #region Posts

        var po1 = new Post(or, ev1, "Ti1", "Po1")
        { Guid = Guid.Parse("00100000-0000-0000-0000-000000000000") };

        var po2 = new Post(or, ev1, "Ti1", "Po1")
        { Guid = Guid.Parse("00200000-0000-0000-0000-000000000000") };

        var po3 = new Post(or, ev3, "Ti3", "Po3")
        { Guid = Guid.Parse("00300000-0000-0000-0000-000000000000") };

        po1.Reactions.Add(new Reaction(st, po1, ReactionType.Like));

        po2.Reactions.Add(new Reaction(st, po2, ReactionType.Sad));

        po3.Reactions.Add(new Reaction(st, po3, ReactionType.Funny));
        po3.Reactions.Add(new Reaction(ad, po3, ReactionType.Wow));

        uow.Repository<Post>().AddMany([po1, po2, po3]);

        #endregion Posts

        #region Comments

        var co1 = new Comment(st, po1, "Co1", null)
        { Guid = Guid.Parse("00010000-0000-0000-0000-000000000000") };

        var co2 = new Comment(st, po3, "Co2", null)
        { Guid = Guid.Parse("00020000-0000-0000-0000-000000000000") };

        var co3 = new Comment(ad, po3, "Co3", co2)
        { Guid = Guid.Parse("00030000-0000-0000-0000-000000000000") };

        var co4 = new Comment(ad, po3, "Co4", null)
        { Guid = Guid.Parse("00040000-0000-0000-0000-000000000000") };

        co1.Likes.Add(new Like(or, co1, false));

        co3.Likes.Add(new Like(st, co3, true));

        co4.Likes.Add(new Like(or, co4, false));
        co4.Likes.Add(new Like(st, co4, false));

        uow.Repository<Comment>().AddMany([co1, co2, co3, co4]);

        #endregion Comments

        #region Reports

        var re1 = new CommentReport(co3, st, "Re1", "Des1")
        { Guid = Guid.Parse("00001000-0000-0000-0000-000000000000") };

        var re2 = new PostReport(po2, ad, "Re2", "Des2")
        { Guid = Guid.Parse("00002000-0000-0000-0000-000000000000") };

        var re3 = new EventReport(ev2, st, "Re3", "Des3")
        { Guid = Guid.Parse("00003000-0000-0000-0000-000000000000") };

        re1.Responder = ad;
        re1.IsOpen = false;

        re3.Responder = ad;

        #endregion Reports

        #region Notifications

        var no1 = new Notification(st, po3, NotificationType.EventNewPost, week[3])
        { Guid = Guid.Parse("00000100-0000-0000-0000-000000000000") };

        var no2 = new Notification(ad, po3, NotificationType.EventNewPost, week[3])
        { Guid = Guid.Parse("00000200-0000-0000-0000-000000000000") };

        var no3 = new Notification(st, co3, NotificationType.CommentRespondedTo, week[3])
        { Guid = Guid.Parse("00000300-0000-0000-0000-000000000000") };

        var no4 = new SocialNotification(or, ad, co4, SocialNotificationType.FriendCommented, week[3])
        { Guid = Guid.Parse("00000400-0000-0000-0000-000000000000") };

        uow.Repository<BaseNotification>().AddMany([no1, no2, no3, no4]);

        #endregion Notifications

        uow.Commit();

        app.Logger.LogInformation("Database was seeded with test data");
    }

    public static void GenerateDevelopmentAdminJwt(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var jwtHandler = scope.ServiceProvider.GetRequiredService<IJwtHandler>();

        TimeSpan kindaMonth = TimeSpan.FromDays(30);

        var devAdmin = uow.Repository<User>().GetAll().Where(x => Equals(x.ExternalId, "DevAdmin")).SingleOrDefault();
        if (devAdmin is not null)
            app.Logger.LogInformation($"DevAdmin's JWT: {jwtHandler.Encode(devAdmin.Guid, kindaMonth)}");

        var devOrganizer = uow.Repository<User>().GetAll().Where(x => Equals(x.ExternalId, "DevOrganizer")).SingleOrDefault();
        if (devOrganizer is not null)
            app.Logger.LogInformation($"DevOrganizer's JWT: {jwtHandler.Encode(devOrganizer.Guid, kindaMonth)}");

        var devStudent = uow.Repository<User>().GetAll().Where(x => Equals(x.ExternalId, "DevStudent")).SingleOrDefault();
        if (devStudent is not null)
            app.Logger.LogInformation($"DevStudent's JWT: {jwtHandler.Encode(devStudent.Guid, kindaMonth)}");
    }
}

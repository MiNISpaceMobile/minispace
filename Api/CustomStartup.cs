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

        if (uow.Repository<Comment>().GetAll().Any() ||
            uow.Repository<Event>().GetAll().Any() ||
            uow.Repository<Post>().GetAll().Any() ||
            uow.Repository<Report>().GetAll().Any() ||
            uow.Repository<User>().GetAll().Any())
        {
            app.Logger.LogInformation("Skipped database seeding since it was not empty");
            return;
        }

        var now = DateTime.Now;
        var week_in = now.AddDays(7);
        var weeks_in = now.AddDays(14);
        var week_ago = now.AddDays(-7);
        var years_ago = now.AddYears(-30).AddMonths(-2).AddDays(3);

        var devAdmin = new User("Dev", "Admin", "dev.admin@pw.edu.pl", now, "DevAdmin")
        { Guid = Guid.Parse("2a4bdafb-c2bd-43d5-9693-b77d4c1ceeb3"), IsAdmin = true, IsOrganizer = true };

        var devOrganizer = new User("Dev", "Organizer", "dev.organizer@pw.edu.pl", now, "DevOrganizer")
        { Guid = Guid.Parse("d48c1100-3358-4734-9cca-05acadb5366e"), IsOrganizer = true };

        var devStudent = new User("Dev", "Student", "dev.student@pw.edu.pl", now, "DevStudent")
        { Guid = Guid.Parse("ca67e06e-9e32-4955-9b3c-02c673322779") };

        uow.Repository<User>().AddMany([devAdmin, devOrganizer, devStudent]);

        var ad0 = new User("AdFirst0", "AdLast0", "ad0@pw.edu.pl", now)
        { Guid = Guid.Parse("d1f78079-dab3-4ee3-b8c2-73b217377d89"), IsAdmin = true };
        var ad1 = new User("AdFirst1", "AdLast1", "ad1@pw.edu.pl", now)
        { Guid = Guid.Parse("2fbe3558-20d7-4872-8048-c6dbdd13a813"), IsAdmin = true };
        var ad2 = new User("AdFirst2", "AdLast2", "ad2@pw.edu.pl", now)
        { Guid = Guid.Parse("01c018c1-9dcc-42ba-9b53-123e33be0db2"), IsAdmin = true };
        
        var st0 = new User("StFirst0", "StLast0", "st0@pw.edu.pl", now)
        { Guid = Guid.Parse("e59669d5-7621-4685-8275-cb3432a1aa0f"), IsOrganizer = true };
        var st1 = new User("StFirst1", "StLast1", "st1@pw.edu.pl", now)
        { Guid = Guid.Parse("1cf19aa7-af25-4129-b01b-507c3816a347"), DateOfBirth = years_ago };
        var st2 = new User("StFirst2", "StLast2", "st2@pw.edu.pl", now)
        { Guid = Guid.Parse("08a5310b-02ec-4166-b102-f38face8cb27"), EmailNotification = false };

        User[] users = [ad0, ad1, ad2, st0, st1, st2];

        var ev0 = new Event(st0, "Ev0", "Des0", EventCategory.Uncategorized, week_ago, now, week_in, "Loc1", null, null)
            { Guid = Guid.Parse("3a788a02-e7e9-4c96-b268-96eb9f71b98c"), ViewCount = 2 };
        var ev1 = new Event(st0, "Ev1", "Des1", EventCategory.Uncategorized, now, week_in, weeks_in, "Loc2", 10, null)
            { Guid = Guid.Parse("22b365d8-c76a-4e22-b546-9e37e5917fe1"), ViewCount = 3 };
        var ev2 = new Event(st1, "Ev2", "Des2", EventCategory.Uncategorized, week_ago, now, weeks_in, "Loc3", null, 10)
            { Guid = Guid.Parse("acbcd26e-877c-45cf-a697-ff4a3c70980a"), ViewCount = 2 };
        ev0.Participants.Add(st0); ev0.Participants.Add(st1);
        st0.JoinedEvents.Add(ev0); st1.JoinedEvents.Add(ev0);
        ev1.Participants.Add(st0);
        ev1.Interested.Add(st2);
        st0.JoinedEvents.Add(ev1); st2.SubscribedEvents.Add(ev1);
        ev2.Interested.Add(st1); ev2.Interested.Add(st2);
        st1.SubscribedEvents.Add(ev2); st2.SubscribedEvents.Add(ev2);
        Event[] events = [ev0, ev1, ev2];

        var po0 = new Post(st0, ev0, "T0", "Po0")
            { Guid = Guid.Parse("0fc79a80-dfdd-4d12-ade4-44f03c0ad9ef") };
        var po1 = new Post(st0, ev0, "T1", "Po1")
            { Guid = Guid.Parse("fe92c735-b049-406d-9a4c-d6d9b4456d4e") };
        var po2 = new Post(st1, ev2, "T2", "Po2")
            { Guid = Guid.Parse("8857657f-3157-4ff4-b06f-b6828e2b7d7e") };
        Post[] posts = [po0, po1, po2];

        var co0 = new Comment(st1, po0, "Co0", null)
            { Guid = Guid.Parse("06954235-f2b9-490e-b8ca-76b567db2ce4") };
        var co1 = new Comment(st0, po0, "Co1", co0)
            { Guid = Guid.Parse("29953098-1ab8-42e6-b07e-e84a50246247") };
        var co2 = new Comment(st2, po2, "Co2", null)
            { Guid = Guid.Parse("09c24709-70dc-4595-83a6-f738fbf6bd7a") };
        Comment[] comments = [co0, co1, co2];

        var re0 = new CommentReport(co2, st1, "Re0", "Des0")
            { Guid = Guid.Parse("05cc7ef1-489b-4313-89fc-f617a07248f9") };
        var re1 = new EventReport(ev0, st0, "Re1", "Des1")
            { Guid = Guid.Parse("7fb6f604-3a5c-4b57-9211-7173776cb0d7") };
        var re2 = new PostReport(po2, st2, "Re2", "Des2")
            { Guid = Guid.Parse("1efef05d-6ee6-48be-b636-dde8f82e8679") };
        Report[] reports = [re0, re1, re2];

        uow.Repository<Comment>().AddMany(comments);
        uow.Repository<Event>().AddMany(events);
        uow.Repository<Post>().AddMany(posts);
        uow.Repository<Report>().AddMany(reports);
        uow.Repository<User>().AddMany(users);

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

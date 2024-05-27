using Domain.Abstractions;
using Domain.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class CustomStartup
{
    public static void PerformCustomStartupActions(this WebApplication app, bool resetDb, bool generateDevAdminJwt)
    {
        if (resetDb)
            app.DeleteDevelopmentDatabase();
        app.CreateOrUpdateDevelopmentDatabase();
        app.SeedDatabaseIfEmpty();
        if (app.Environment.IsDevelopment() && generateDevAdminJwt)
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

        if (app.Environment.IsDevelopment())
        {
            var devAdmin = new User("Dev", "Admin", "dev.admin@pw.edu.pl", now, "DevAdmin")
            { Guid = Guid.Parse("2a4bdafb-c2bd-43d5-9693-b77d4c1ceeb3"), IsAdmin = true };
            uow.Repository<User>().Add(devAdmin);
        }

        var ad0 = new User("AdFirst0", "AdLast0", "ad0@pw.edu.pl", now) { IsAdmin = true };
        var ad1 = new User("AdFirst1", "AdLast1", "ad1@pw.edu.pl", now) { IsAdmin = true };
        var ad2 = new User("AdFirst2", "AdLast2", "ad2@pw.edu.pl", now) { IsAdmin = true };
        
        var st0 = new User("StFirst0", "StLast0", "st0@pw.edu.pl", now) { IsOrganizer = true };
        var st1 = new User("StFirst1", "StLast1", "st1@pw.edu.pl", now) { DateOfBirth = years_ago };
        var st2 = new User("StFirst2", "StLast2", "st2@pw.edu.pl", now) { EmailNotification = false };
        st0.Friends.Add(st2); st2.Friends.Add(st0);

        User[] users = [ad0, ad1, ad2, st0, st1, st2];

        var ev0 = new Event(st0, "Ev0", "Des0", EventCategory.Uncategorized, week_ago, now, week_in, "Loc1", null, null) { ViewCount = 2 };
        var ev1 = new Event(st0, "Ev1", "Des1", EventCategory.Uncategorized, now, week_in, weeks_in, "Loc2", 10, null) { ViewCount = 3 };
        var ev2 = new Event(st1, "Ev2", "Des2", EventCategory.Uncategorized, week_ago, now, weeks_in, "Loc3", null, 10) { ViewCount = 2 };
        ev0.Participants.Add(st0); ev0.Participants.Add(st1);
        ev0.Interested.Add(st0);
        ev1.Participants.Add(st0);
        ev1.Interested.Add(st0); ev1.Interested.Add(st2);
        ev2.Participants.Add(st1);
        ev2.Interested.Add(st1); ev2.Interested.Add(st2);
        Event[] events = [ev0, ev1, ev2];

        var po0 = new Post(st0, ev0, "Po0");
        var po1 = new Post(st0, ev0, "Po1");
        var po2 = new Post(st1, ev2, "Po2");
        Post[] posts = [po0, po1, po2];

        var co0 = new Comment(st1, po0, "Co0", null);
        var co1 = new Comment(st0, po0, "Co1", co0);
        var co2 = new Comment(st2, po2, "Co2", null);
        co1.Likers.Add(st1);
        Comment[] comments = [co0, co1, co2];

        var re0 = new CommentReport(co2, st1, "Re0", "Des0");
        var re1 = new EventReport(ev0, st0, "Re1", "Des1");
        var re2 = new PostReport(po2, st2, "Re2", "Des2");
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
        var devAdmin = scope.ServiceProvider.GetRequiredService<IUnitOfWork>().Repository<User>()
            .GetAll().Where(x => Equals(x.ExternalId, "DevAdmin")).SingleOrDefault();
        if (devAdmin is null)
            return;
        var jwt = scope.ServiceProvider.GetRequiredService<IJwtHandler>().Encode(devAdmin.Guid);
        app.Logger.LogDebug($"DevAdmin's JWT: {jwt}");
    }
}

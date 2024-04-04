using Domain.Abstractions;
using Domain.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class AppCustomStartup
{
    public static void PerformCustomStartupActions(this WebApplication app, bool resetDb)
    {
        if (resetDb)
            app.DeleteDevelopmentDatabase();
        app.CreateOrUpdateDevelopmentDatabase();
        app.SeedDatabaseIfEmpty();
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

        if (uow.Repository<Comment>().GetAll().Count() > 0 ||
            uow.Repository<Event>().GetAll().Count() > 0 ||
            uow.Repository<Post>().GetAll().Count() > 0 ||
            uow.Repository<Report>().GetAll().Count() > 0 ||
            uow.Repository<User>().GetAll().Count() > 0)
            return;

        var now = DateTime.Now;
        var week_in = now.AddDays(7);
        var weeks_in = now.AddDays(14);
        var week_ago = now.AddDays(-7);
        var years_ago = now.AddYears(-30).AddMonths(-2).AddDays(3);

        var ad0 = new Administrator("Ad0", "ad0@pw.edu.pl", "ad0");
        var ad1 = new Administrator("Ad1", "ad1@pw.edu.pl", "ad1");
        var ad2 = new Administrator("Ad2", "ad2@pw.edu.pl", "ad2");
        Administrator[] administrators = [ad0, ad1, ad2];

        var st0 = new Student("St0", "st0@pw.edu.pl", "st0") { IsOrganizer = true };
        var st1 = new Student("St1", "st1@pw.edu.pl", "st1") { DateOfBirth = years_ago };
        var st2 = new Student("St2", "st2@pw.edu.pl", "st2") { EmailNotification = false };
        st0.Friends.Add(st2); st2.Friends.Add(st0);
        Student[] students = [st0, st1, st2];

        var ev0 = new Event(st0, "Ev0", "Test0", EventCategory.Uncategorized, week_ago, now, week_in, "Loc1", null, null) { ViewCount = 2 };
        var ev1 = new Event(st0, "Ev1", "Test1", EventCategory.Uncategorized, now, week_in, weeks_in, "Loc2", 10, null) { ViewCount = 3 };
        var ev2 = new Event(st1, "Ev2", "Test2", EventCategory.Uncategorized, week_ago, now, weeks_in, "Loc3", null, 10) { ViewCount = 2 };
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
        Comment[] comments = [co0, co1, co2];

        Report[] reports = [];

        uow.Repository<Administrator>().AddMany(administrators);
        uow.Repository<Comment>().AddMany(comments);
        uow.Repository<Event>().AddMany(events);
        uow.Repository<Post>().AddMany(posts);
        uow.Repository<Report>().AddMany(reports);
        uow.Repository<Student>().AddMany(students);
    }
}

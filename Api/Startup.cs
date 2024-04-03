using Domain.Abstractions;
using Domain.DataModel;
using Infrastructure.DatabaseContexts;
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
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        if (uow.Repository<Comment>().GetAll().Count() > 0 ||
            uow.Repository<Event>().GetAll().Count() > 0 ||
            uow.Repository<Post>().GetAll().Count() > 0 ||
            uow.Repository<Report>().GetAll().Count() > 0 ||
            uow.Repository<User>().GetAll().Count() > 0)
            return;

        Administrator[] administrators = new[]
        {
            new Administrator("Adminiak1", "ad1@pw.edu.pl", "ad1"),
            new Administrator("Adminiak2", "ad2@pw.edu.pl", "ad2"),
            new Administrator("Adminiak3", "ad3@pw.edu.pl", "ad3"),
        };

        Student[] students = new[]
        {
            new Student("Studenciak1", "st1@pw.edu.pl", "st1"),
            new Student("Studenciak2", "st2@pw.edu.pl", "st2"),
            new Student("Studenciak3", "st3@pw.edu.pl", "st3"),
        };


    }
}

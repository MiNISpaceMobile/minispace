using Domain.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DatabaseContexts;

// For development
public class SqliteDbContext : DbContext
{
    public const string DbFolderName = "MinispaceDb";
    public const string DbFileName = "sqlite.db";

    public static readonly string DbFolderPath;
    public static readonly string DbFilePath;

    static SqliteDbContext()
    {
        DbFolderPath = Path.Join(Directory.GetCurrentDirectory(), DbFolderName);
        if (!Directory.Exists(DbFolderPath))
            Directory.CreateDirectory(DbFolderPath);
        DbFilePath = Path.Join(DbFolderPath, DbFileName);
    }

    public void CreateOrUpdate() => Database.Migrate();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbFilePath}");

    protected override void OnModelCreating(ModelBuilder model) => model.Configure();
}
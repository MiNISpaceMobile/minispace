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
        DbFilePath = Path.Join(DbFolderPath, DbFileName);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbFilePath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => EntityFrameworkConfiguration.Configure(modelBuilder);
}
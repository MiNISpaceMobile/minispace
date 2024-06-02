using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DatabaseContexts;

// For production
public class AzureMySqlDbContext : DbContext
{
    private string connectionString;

    public AzureMySqlDbContext(IConfiguration config)
    {
        connectionString = config["AZURE_MYSQL_CONNECTIONSTRING"]!;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseMySql(ServerVersion.AutoDetect(connectionString)).Configure();

    protected override void OnModelCreating(ModelBuilder model) => model.Configure();
}

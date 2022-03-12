using Microsoft.EntityFrameworkCore;

namespace DotInfo.Model;

public class DotInfoDbContext : DbContext
{
    private readonly string _connectionString;

    public DotInfoDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .UseLowerCaseNamingConvention();
    }
    
    public DbSet<DotInfo> DotInfo { get; set; }
    
}
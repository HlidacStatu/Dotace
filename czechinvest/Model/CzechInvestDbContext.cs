using Microsoft.EntityFrameworkCore;

namespace CzechInvest.Model;

public class CzechInvestDbContext : DbContext
{
    private readonly string _connectionString;

    public CzechInvestDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .UseLowerCaseNamingConvention();
    }
    
    public DbSet<CzechInvest> CzechInvest { get; set; }
    
}
using Microsoft.EntityFrameworkCore;

namespace Szif.Model;

public class SzifDbContext : DbContext
{
    private readonly string _connectionString;

    public SzifDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .UseLowerCaseNamingConvention();
    }
    
    public DbSet<Szif> Szif { get; set; }
    
}
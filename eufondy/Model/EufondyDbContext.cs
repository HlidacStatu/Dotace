using Microsoft.EntityFrameworkCore;

namespace Eufondy.Model;

public class EufondyDbContext : DbContext
{
    private readonly string _connectionString;

    public EufondyDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .UseLowerCaseNamingConvention();
    }
    
    public DbSet<Dotace2006> Dotace06 { get; set; }
    public DbSet<Dotace2013> Dotace13 { get; set; }
    public DbSet<Dotace2020> Dotace20 { get; set; }
   
}
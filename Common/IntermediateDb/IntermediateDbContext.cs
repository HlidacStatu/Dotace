using Microsoft.EntityFrameworkCore;

namespace Common.IntermediateDb;

public class IntermediateDbContext : DbContext
{
    private readonly string _connectionString;

    public IntermediateDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(_connectionString)
            .UseLowerCaseNamingConvention();
    }
    
    public DbSet<Dotace> Dotace { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("dotace");
    }
}
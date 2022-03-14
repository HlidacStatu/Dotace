using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Common.IntermediateDb;

public class DesignTimeContextFactory : IDesignTimeDbContextFactory<IntermediateDbContext>
{
    public IntermediateDbContext CreateDbContext(string[] args)
    {
        return new IntermediateDbContext(@"connectionstring");
    }
}

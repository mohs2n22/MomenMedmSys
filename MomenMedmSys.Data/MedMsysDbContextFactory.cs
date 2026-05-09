using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MomenMedmSys.Data;

public class MedMsysDbContextFactory : IDesignTimeDbContextFactory<MedMsysDbContext>
{
    public MedMsysDbContext CreateDbContext(string[] args)
    {
        var dbPath = args.Length > 0 ? args[0] : "medmsys.db";
        var connectionString = $"Data Source={dbPath}";

        var optionsBuilder = new DbContextOptionsBuilder<MedMsysDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new MedMsysDbContext(optionsBuilder.Options);
    }
}

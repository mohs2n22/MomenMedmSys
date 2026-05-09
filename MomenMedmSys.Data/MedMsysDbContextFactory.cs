using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MomenMedmSys.Data;

namespace MomenMedmSys.Data;

public class MedMsysDbContextFactory : IDesignTimeDbContextFactory<MedMsysDbContext>
{
    public MedMsysDbContext CreateDbContext(string[] args)
    {
        // Design-time connection string for EF Core migrations
        // Runtime connection is loaded from appsettings.json in Program.cs
        var connectionString = args.Length > 0
            ? args[0]
            : "Server=localhost;Database=medmsys;User=Anony;Password=4565;SslMode=None;AllowPublicKeyRetrieval=True;";

        var optionsBuilder = new DbContextOptionsBuilder<MedMsysDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString));

        return new MedMsysDbContext(optionsBuilder.Options);
    }
}

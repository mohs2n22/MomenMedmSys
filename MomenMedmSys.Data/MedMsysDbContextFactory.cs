using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MomenMedmSys.Data
{
    public class MedMsysDbContextFactory : IDesignTimeDbContextFactory<MedMsysDbContext>
    {
        public MedMsysDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MedMsysDbContext>();
            optionsBuilder.UseSqlite("Data Source=medmsys.db");
            return new MedMsysDbContext(optionsBuilder.Options);
        }
    }
}

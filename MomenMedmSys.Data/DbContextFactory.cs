using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MomenMedmSys.Data
{
    public static class DbContextFactory
    {
        public static IServiceCollection AddMedMsysDbContext(this IServiceCollection services, AppConfig config)
        {
            services.AddDbContext<MedMsysDbContext>(options =>
            {
                options.UseSqlite(config.Database.ConnectionString);
            });
            return services;
        }
    }
}

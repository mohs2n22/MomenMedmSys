using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;
using MomenMedmSys.Data.Repositories;

namespace MomenMedmSys.Services
{
    public class HospitalSettingsService : IHospitalSettingsService
    {
        private readonly MedMsysDbContext _context;
        private readonly IRepository<HospitalSettings> _settingsRepo;

        public HospitalSettingsService(MedMsysDbContext context, IRepository<HospitalSettings> settingsRepo)
        {
            _context = context;
            _settingsRepo = settingsRepo;
        }

        public async Task<HospitalSettings?> GetSettingsAsync()
        {
            return await _context.HospitalSettings.FirstOrDefaultAsync();
        }

        public async Task<HospitalSettings> SaveSettingsAsync(HospitalSettings settings)
        {
            var existing = await GetSettingsAsync();
            if (existing != null)
            {
                existing.HospitalName = settings.HospitalName;
                existing.LogoPath = settings.LogoPath;
                existing.UpdatedAt = System.DateTime.Now;
                _settingsRepo.Update(existing);
                return existing;
            }
            else
            {
                settings.CreatedAt = System.DateTime.Now;
                settings.IsActive = true;
                await _settingsRepo.AddAsync(settings);
                return settings;
            }
        }

        public async Task<string> GetHospitalNameAsync()
        {
            var settings = await GetSettingsAsync();
            return settings?.HospitalName ?? "MomenMedmSys";
        }

        public async Task<string> GetLogoPathAsync()
        {
            var settings = await GetSettingsAsync();
            return settings?.LogoPath ?? string.Empty;
        }
    }
}
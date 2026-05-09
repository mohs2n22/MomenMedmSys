using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;

namespace MomenMedmSys.Services
{
    public interface IHospitalSettingsService
    {
        Task<HospitalSettings?> GetSettingsAsync();
        Task<HospitalSettings> SaveSettingsAsync(HospitalSettings settings);
        Task<string> GetHospitalNameAsync();
        Task<string> GetLogoPathAsync();
    }
}
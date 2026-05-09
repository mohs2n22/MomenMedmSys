using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly IHospitalSettingsService _settingsService;

        public AdminModel(IHospitalSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [BindProperty]
        public HospitalSettings HospitalSettings { get; set; } = new();

        public string? Message { get; set; }
        public bool MessageSuccess { get; set; }

        public async Task OnGetAsync()
        {
            var settings = await _settingsService.GetSettingsAsync();
            if (settings != null)
            {
                HospitalSettings = settings;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(HospitalSettings.HospitalName))
            {
                Message = "Hospital name is required.";
                MessageSuccess = false;
                return Page();
            }

            HospitalSettings.IsActive = true;
            await _settingsService.SaveSettingsAsync(HospitalSettings);
            Message = "Hospital settings saved successfully.";
            MessageSuccess = true;
            return Page();
        }
    }
}
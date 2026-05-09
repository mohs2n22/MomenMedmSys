using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomenMedmSys.Web.Pages
{
    [Authorize]
    public class UsersModel : PageModel
    {
        private readonly IStaffManagementService _staffService;

        public UsersModel(IStaffManagementService staffService)
        {
            _staffService = staffService;
        }

        public List<StaffMember> StaffList { get; set; } = new List<StaffMember>();
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync(string search = "")
        {
            SearchTerm = search;
            var allStaff = await _staffService.GetAllStaffAsync();
            StaffList = string.IsNullOrEmpty(search)
                ? allStaff.ToList()
                : allStaff.Where(s =>
                    s.FullName.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    s.JobTitle.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    s.Department.Contains(search, System.StringComparison.OrdinalIgnoreCase)
                ).ToList();
        }
    }
}

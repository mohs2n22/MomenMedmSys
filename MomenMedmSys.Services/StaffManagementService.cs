using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    public interface IStaffManagementService
    {
        Task<IEnumerable<StaffMember>> GetAllStaffAsync();
        Task<IEnumerable<StaffMember>> GetStaffByRoleAsync(StaffRole role);
        Task<IEnumerable<StaffMember>> GetActiveStaffAsync();
        Task<IEnumerable<StaffMember>> GetStaffByDepartmentAsync(string department);
        Task<StaffMember?> GetStaffByIdAsync(int id);
        Task<StaffMember?> GetStaffByUsernameAsync(string username);
        Task<StaffMember> CreateStaffAsync(StaffMember staff);
        Task UpdateStaffAsync(StaffMember staff);
        Task DeleteStaffAsync(int id);
        Task<bool> ResetPasswordAsync(int staffId, string newPasswordHash);
        Task<bool> ToggleAccountLockAsync(int staffId, bool locked);
        Task UpdateLastLoginAsync(int staffId);
        Task<int> GetActiveAccountCountAsync();
        Task<int> GetLockedAccountCountAsync();

        // Role-specific queries
        Task<IEnumerable<StaffMember>> GetAdministratorsAsync();
        Task<IEnumerable<StaffMember>> GetHardwareTechniciansAsync();
        Task<IEnumerable<StaffMember>> GetReportWritersAsync();
        Task<IEnumerable<StaffMember>> GetPhysiciansAsync();
        Task<IEnumerable<StaffMember>> GetNursesAsync();
    }

    public class StaffManagementService : IStaffManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StaffMember>> GetAllStaffAsync()
        {
            return await _unitOfWork.StaffMembers.GetAllAsync();
        }

        public async Task<IEnumerable<StaffMember>> GetStaffByRoleAsync(StaffRole role)
        {
            return await _unitOfWork.StaffMembers.FindAsync(s => s.Role == role);
        }

        public async Task<IEnumerable<StaffMember>> GetActiveStaffAsync()
        {
            return await _unitOfWork.StaffMembers.FindAsync(s => s.IsActive && !s.TerminationDate.HasValue);
        }

        public async Task<IEnumerable<StaffMember>> GetStaffByDepartmentAsync(string department)
        {
            return await _unitOfWork.StaffMembers.FindAsync(s => s.Department == department && s.IsActive);
        }

        public async Task<StaffMember?> GetStaffByIdAsync(int id)
        {
            return await _unitOfWork.StaffMembers.GetByIdAsync(id);
        }

        public async Task<StaffMember?> GetStaffByUsernameAsync(string username)
        {
            return await _unitOfWork.StaffMembers.FirstOrDefaultAsync(s => s.Username == username);
        }

        public async Task<StaffMember> CreateStaffAsync(StaffMember staff)
        {
            staff.CreatedAt = DateTime.Now;
            staff.IsActiveAccount = true;
            await _unitOfWork.StaffMembers.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();
            return staff;
        }

        public async Task UpdateStaffAsync(StaffMember staff)
        {
            staff.UpdatedAt = DateTime.Now;
            _unitOfWork.StaffMembers.Update(staff);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteStaffAsync(int id)
        {
            var staff = await _unitOfWork.StaffMembers.GetByIdAsync(id);
            if (staff != null)
            {
                _unitOfWork.StaffMembers.Remove(staff);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> ResetPasswordAsync(int staffId, string newPasswordHash)
        {
            var staff = await GetStaffByIdAsync(staffId);
            if (staff == null) return false;

            staff.PasswordHash = newPasswordHash;
            staff.FailedLoginAttempts = 0;
            staff.IsLocked = false;
            staff.UpdatedAt = DateTime.Now;
            _unitOfWork.StaffMembers.Update(staff);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleAccountLockAsync(int staffId, bool locked)
        {
            var staff = await GetStaffByIdAsync(staffId);
            if (staff == null) return false;

            staff.IsLocked = locked;
            staff.FailedLoginAttempts = locked ? 0 : staff.FailedLoginAttempts;
            _unitOfWork.StaffMembers.Update(staff);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task UpdateLastLoginAsync(int staffId)
        {
            var staff = await GetStaffByIdAsync(staffId);
            if (staff != null)
            {
                staff.LastLoginDate = DateTime.Now;
                staff.FailedLoginAttempts = 0;
                _unitOfWork.StaffMembers.Update(staff);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<int> GetActiveAccountCountAsync()
        {
            var staff = await GetAllStaffAsync();
            return staff.Count(s => s.IsActiveAccount && !s.IsLocked);
        }

        public async Task<int> GetLockedAccountCountAsync()
        {
            var staff = await GetAllStaffAsync();
            return staff.Count(s => s.IsLocked);
        }

        public async Task<IEnumerable<StaffMember>> GetAdministratorsAsync()
        {
            return await GetStaffByRoleAsync(StaffRole.Administrator);
        }

        public async Task<IEnumerable<StaffMember>> GetHardwareTechniciansAsync()
        {
            return await GetStaffByRoleAsync(StaffRole.HardwareTechnician);
        }

        public async Task<IEnumerable<StaffMember>> GetReportWritersAsync()
        {
            return await GetStaffByRoleAsync(StaffRole.ReportWriter);
        }

        public async Task<IEnumerable<StaffMember>> GetPhysiciansAsync()
        {
            return await GetStaffByRoleAsync(StaffRole.Physician);
        }

        public async Task<IEnumerable<StaffMember>> GetNursesAsync()
        {
            return await GetStaffByRoleAsync(StaffRole.Nurse);
        }
    }
}

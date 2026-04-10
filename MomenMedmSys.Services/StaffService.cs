using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffMember>> GetAllStaffAsync();
        Task<StaffMember?> GetStaffByIdAsync(int id);
        Task<StaffMember> CreateStaffAsync(StaffMember staff);
        Task UpdateStaffAsync(StaffMember staff);
        Task DeleteStaffAsync(int id);
        Task<IEnumerable<StaffMember>> GetStaffByDepartmentAsync(string department);
        Task<IEnumerable<StaffMember>> GetActiveStaffAsync();
    }

    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StaffMember>> GetAllStaffAsync()
        {
            return await _unitOfWork.StaffMembers.GetAllAsync();
        }

        public async Task<StaffMember?> GetStaffByIdAsync(int id)
        {
            return await _unitOfWork.StaffMembers.GetByIdAsync(id);
        }

        public async Task<StaffMember> CreateStaffAsync(StaffMember staff)
        {
            staff.CreatedAt = DateTime.Now;
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

        public async Task<IEnumerable<StaffMember>> GetStaffByDepartmentAsync(string department)
        {
            return await _unitOfWork.StaffMembers.FindAsync(s => s.Department == department && s.IsActive);
        }

        public async Task<IEnumerable<StaffMember>> GetActiveStaffAsync()
        {
            return await _unitOfWork.StaffMembers.FindAsync(s => s.IsActive && !s.TerminationDate.HasValue);
        }
    }

    public interface ITrainingService
    {
        Task<IEnumerable<TrainingRecord>> GetAllTrainingRecordsAsync();
        Task<IEnumerable<TrainingRecord>> GetTrainingByStaffIdAsync(int staffId);
        Task<IEnumerable<TrainingRecord>> GetTrainingByDeviceIdAsync(int deviceId);
        Task<TrainingRecord> CreateTrainingAsync(TrainingRecord record);
        Task UpdateTrainingAsync(TrainingRecord record);
        Task DeleteTrainingAsync(int id);
        Task<IEnumerable<TrainingRecord>> GetExpiredTrainingAsync();
        Task<IEnumerable<TrainingRecord>> GetExpiringSoonTrainingAsync(int daysThreshold = 30);
    }

    public class TrainingService : ITrainingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrainingRecord>> GetAllTrainingRecordsAsync()
        {
            return await _unitOfWork.TrainingRecords.GetAllAsync();
        }

        public async Task<IEnumerable<TrainingRecord>> GetTrainingByStaffIdAsync(int staffId)
        {
            return await _unitOfWork.TrainingRecords.FindAsync(t => t.StaffMemberId == staffId);
        }

        public async Task<IEnumerable<TrainingRecord>> GetTrainingByDeviceIdAsync(int deviceId)
        {
            return await _unitOfWork.TrainingRecords.FindAsync(t => t.DeviceId == deviceId);
        }

        public async Task<TrainingRecord> CreateTrainingAsync(TrainingRecord record)
        {
            record.CreatedAt = DateTime.Now;
            await _unitOfWork.TrainingRecords.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();
            return record;
        }

        public async Task UpdateTrainingAsync(TrainingRecord record)
        {
            record.UpdatedAt = DateTime.Now;
            _unitOfWork.TrainingRecords.Update(record);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTrainingAsync(int id)
        {
            var record = await _unitOfWork.TrainingRecords.GetByIdAsync(id);
            if (record != null)
            {
                _unitOfWork.TrainingRecords.Remove(record);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TrainingRecord>> GetExpiredTrainingAsync()
        {
            var all = await _unitOfWork.TrainingRecords.GetAllAsync();
            return all.Where(t => t.IsExpired);
        }

        public async Task<IEnumerable<TrainingRecord>> GetExpiringSoonTrainingAsync(int daysThreshold = 30)
        {
            var all = await _unitOfWork.TrainingRecords.GetAllAsync();
            return all.Where(t => t.IsExpiringSoon && !t.IsExpired);
        }
    }
}

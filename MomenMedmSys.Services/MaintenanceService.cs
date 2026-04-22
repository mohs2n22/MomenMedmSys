using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for preventive and corrective maintenance scheduling — record management, overdue/upcoming queries,
    /// cost tracking per device, and maintenance type/status filtering.
    /// Supports the Maintenance module with both preventive and corrective workflows.
    /// </summary>
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceRecord>> GetAllRecordsAsync();
        Task<IEnumerable<MaintenanceRecord>> GetRecordsByDeviceIdAsync(int deviceId);
        Task<MaintenanceRecord?> GetRecordByIdAsync(int id);
        Task<MaintenanceRecord> CreateRecordAsync(MaintenanceRecord record);
        Task UpdateRecordAsync(MaintenanceRecord record);
        Task DeleteRecordAsync(int id);
        Task<IEnumerable<MaintenanceRecord>> GetOverdueMaintenanceAsync();
        Task<IEnumerable<MaintenanceRecord>> GetUpcomingMaintenanceAsync(int daysAhead = 7);
        Task<IEnumerable<MaintenanceRecord>> GetRecordsByTypeAsync(MaintenanceType type);
        Task<IEnumerable<MaintenanceRecord>> GetRecordsByStatusAsync(MaintenanceStatus status);
        Task<decimal> GetTotalMaintenanceCostAsync(int deviceId);
        Task<int> GetOverdueCountAsync();
        Task<int> GetScheduledCountAsync();
    }

    public class MaintenanceService : IMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaintenanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetAllRecordsAsync()
        {
            return await _unitOfWork.MaintenanceRecords.GetAllAsync();
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetRecordsByDeviceIdAsync(int deviceId)
        {
            return await _unitOfWork.MaintenanceRecords.FindAsync(m => m.DeviceId == deviceId);
        }

        public async Task<MaintenanceRecord?> GetRecordByIdAsync(int id)
        {
            return await _unitOfWork.MaintenanceRecords.GetByIdAsync(id);
        }

        public async Task<MaintenanceRecord> CreateRecordAsync(MaintenanceRecord record)
        {
            record.CreatedAt = DateTime.Now;
            await _unitOfWork.MaintenanceRecords.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();

            // Update device's last maintenance date
            var device = await _unitOfWork.MedicalDevices.GetByIdAsync(record.DeviceId);
            if (device != null)
            {
                device.LastMaintenanceDate = record.CompletedDate ?? record.ScheduledDate;
                _unitOfWork.MedicalDevices.Update(device);
                await _unitOfWork.SaveChangesAsync();
            }

            return record;
        }

        public async Task UpdateRecordAsync(MaintenanceRecord record)
        {
            record.UpdatedAt = DateTime.Now;
            _unitOfWork.MaintenanceRecords.Update(record);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRecordAsync(int id)
        {
            var record = await _unitOfWork.MaintenanceRecords.GetByIdAsync(id);
            if (record != null)
            {
                _unitOfWork.MaintenanceRecords.Remove(record);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetOverdueMaintenanceAsync()
        {
            var now = DateTime.Now;
            return await _unitOfWork.MaintenanceRecords.FindAsync(m =>
                m.Status == MaintenanceStatus.Scheduled &&
                m.ScheduledDate < now);
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetUpcomingMaintenanceAsync(int daysAhead = 7)
        {
            var now = DateTime.Now;
            var future = now.AddDays(daysAhead);
            return await _unitOfWork.MaintenanceRecords.FindAsync(m =>
                m.Status == MaintenanceStatus.Scheduled &&
                m.ScheduledDate >= now &&
                m.ScheduledDate <= future);
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetRecordsByTypeAsync(MaintenanceType type)
        {
            return await _unitOfWork.MaintenanceRecords.FindAsync(m => m.Type == type);
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetRecordsByStatusAsync(MaintenanceStatus status)
        {
            return await _unitOfWork.MaintenanceRecords.FindAsync(m => m.Status == status);
        }

        public async Task<decimal> GetTotalMaintenanceCostAsync(int deviceId)
        {
            var records = await _unitOfWork.MaintenanceRecords.FindAsync(m => m.DeviceId == deviceId);
            return records.Sum(m => m.TotalCost);
        }

        public async Task<int> GetOverdueCountAsync()
        {
            var overdue = await GetOverdueMaintenanceAsync();
            return overdue.Count();
        }

        public async Task<int> GetScheduledCountAsync()
        {
            var records = await _unitOfWork.MaintenanceRecords.FindAsync(m => m.Status == MaintenanceStatus.Scheduled);
            return records.Count();
        }
    }
}

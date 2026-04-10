using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    public interface ICalibrationService
    {
        Task<IEnumerable<CalibrationRecord>> GetAllRecordsAsync();
        Task<IEnumerable<CalibrationRecord>> GetRecordsByDeviceIdAsync(int deviceId);
        Task<CalibrationRecord?> GetRecordByIdAsync(int id);
        Task<CalibrationRecord> CreateRecordAsync(CalibrationRecord record);
        Task UpdateRecordAsync(CalibrationRecord record);
        Task DeleteRecordAsync(int id);
        Task<IEnumerable<CalibrationRecord>> GetOverdueCalibrationsAsync();
        Task<IEnumerable<CalibrationRecord>> GetUpcomingCalibrationsAsync(int daysAhead = 30);
        Task<IEnumerable<CalibrationRecord>> GetRecordsByResultAsync(CalibrationResult result);
        Task<int> GetOverdueCountAsync();
        Task<int> GetPassCountAsync();
        Task<int> GetFailCountAsync();
    }

    public class CalibrationService : ICalibrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalibrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CalibrationRecord>> GetAllRecordsAsync()
        {
            return await _unitOfWork.CalibrationRecords.GetAllAsync();
        }

        public async Task<IEnumerable<CalibrationRecord>> GetRecordsByDeviceIdAsync(int deviceId)
        {
            return await _unitOfWork.CalibrationRecords.FindAsync(c => c.DeviceId == deviceId);
        }

        public async Task<CalibrationRecord?> GetRecordByIdAsync(int id)
        {
            return await _unitOfWork.CalibrationRecords.GetByIdAsync(id);
        }

        public async Task<CalibrationRecord> CreateRecordAsync(CalibrationRecord record)
        {
            record.CreatedAt = DateTime.Now;
            await _unitOfWork.CalibrationRecords.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();

            // Update device's last calibration date
            var device = await _unitOfWork.MedicalDevices.GetByIdAsync(record.DeviceId);
            if (device != null)
            {
                device.LastCalibrationDate = record.CalibrationDate;
                if (record.Result == CalibrationResult.Pass || record.Result == CalibrationResult.PassWithAdjustment)
                {
                    device.Status = DeviceStatus.Active;
                }
                _unitOfWork.MedicalDevices.Update(device);
                await _unitOfWork.SaveChangesAsync();
            }

            return record;
        }

        public async Task UpdateRecordAsync(CalibrationRecord record)
        {
            record.UpdatedAt = DateTime.Now;
            _unitOfWork.CalibrationRecords.Update(record);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteRecordAsync(int id)
        {
            var record = await _unitOfWork.CalibrationRecords.GetByIdAsync(id);
            if (record != null)
            {
                _unitOfWork.CalibrationRecords.Remove(record);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CalibrationRecord>> GetOverdueCalibrationsAsync()
        {
            var now = DateTime.Now;
            return await _unitOfWork.CalibrationRecords.FindAsync(c =>
                c.NextDueDate < now);
        }

        public async Task<IEnumerable<CalibrationRecord>> GetUpcomingCalibrationsAsync(int daysAhead = 30)
        {
            var now = DateTime.Now;
            var future = now.AddDays(daysAhead);
            return await _unitOfWork.CalibrationRecords.FindAsync(c =>
                c.NextDueDate >= now &&
                c.NextDueDate <= future);
        }

        public async Task<IEnumerable<CalibrationRecord>> GetRecordsByResultAsync(CalibrationResult result)
        {
            return await _unitOfWork.CalibrationRecords.FindAsync(c => c.Result == result);
        }

        public async Task<int> GetOverdueCountAsync()
        {
            var overdue = await GetOverdueCalibrationsAsync();
            return overdue.Count();
        }

        public async Task<int> GetPassCountAsync()
        {
            var records = await _unitOfWork.CalibrationRecords.FindAsync(c =>
                c.Result == CalibrationResult.Pass || c.Result == CalibrationResult.PassWithAdjustment);
            return records.Count();
        }

        public async Task<int> GetFailCountAsync()
        {
            var records = await _unitOfWork.CalibrationRecords.FindAsync(c =>
                c.Result == CalibrationResult.Fail || c.Result == CalibrationResult.OutOfTolerance || c.Result == CalibrationResult.NotCalibrated);
            return records.Count();
        }
    }
}

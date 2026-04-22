using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for electrical safety test management — test CRUD, overdue/due-soon detection,
    /// pass/fail tracking, and compliance verification for medical devices.
    /// </summary>
    public interface IElectricalSafetyService
    {
        Task<IEnumerable<ElectricalSafetyTest>> GetAllTestsAsync();
        Task<IEnumerable<ElectricalSafetyTest>> GetTestsByDeviceIdAsync(int deviceId);
        Task<ElectricalSafetyTest?> GetTestByIdAsync(int id);
        Task<ElectricalSafetyTest> CreateTestAsync(ElectricalSafetyTest test);
        Task UpdateTestAsync(ElectricalSafetyTest test);
        Task DeleteTestAsync(int id);
        Task<IEnumerable<ElectricalSafetyTest>> GetOverdueTestsAsync();
        Task<IEnumerable<ElectricalSafetyTest>> GetTestsDueSoonAsync(int daysThreshold = 30);
        Task<IEnumerable<ElectricalSafetyTest>> GetFailedTestsAsync();
        Task<int> GetOverdueCountAsync();
        Task<int> GetPassCountAsync();
        Task<int> GetFailCountAsync();
    }

    public class ElectricalSafetyService : IElectricalSafetyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ElectricalSafetyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ElectricalSafetyTest>> GetAllTestsAsync()
        {
            return await _unitOfWork.ElectricalSafetyTests.GetAllAsync();
        }

        public async Task<IEnumerable<ElectricalSafetyTest>> GetTestsByDeviceIdAsync(int deviceId)
        {
            return await _unitOfWork.ElectricalSafetyTests.FindAsync(e => e.DeviceId == deviceId);
        }

        public async Task<ElectricalSafetyTest?> GetTestByIdAsync(int id)
        {
            return await _unitOfWork.ElectricalSafetyTests.GetByIdAsync(id);
        }

        public async Task<ElectricalSafetyTest> CreateTestAsync(ElectricalSafetyTest test)
        {
            test.CreatedAt = DateTime.Now;
            await _unitOfWork.ElectricalSafetyTests.AddAsync(test);
            await _unitOfWork.SaveChangesAsync();

            // Update device's last safety test date
            var device = await _unitOfWork.MedicalDevices.GetByIdAsync(test.DeviceId);
            if (device != null)
            {
                device.LastSafetyTestDate = test.TestDate;
                _unitOfWork.MedicalDevices.Update(device);
                await _unitOfWork.SaveChangesAsync();
            }

            return test;
        }

        public async Task UpdateTestAsync(ElectricalSafetyTest test)
        {
            test.UpdatedAt = DateTime.Now;
            _unitOfWork.ElectricalSafetyTests.Update(test);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTestAsync(int id)
        {
            var test = await _unitOfWork.ElectricalSafetyTests.GetByIdAsync(id);
            if (test != null)
            {
                _unitOfWork.ElectricalSafetyTests.Remove(test);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ElectricalSafetyTest>> GetOverdueTestsAsync()
        {
            var all = await _unitOfWork.ElectricalSafetyTests.GetAllAsync();
            return all.Where(e => e.IsOverdue);
        }

        public async Task<IEnumerable<ElectricalSafetyTest>> GetTestsDueSoonAsync(int daysThreshold = 30)
        {
            var all = await _unitOfWork.ElectricalSafetyTests.GetAllAsync();
            return all.Where(e => e.IsDueSoon && !e.IsOverdue);
        }

        public async Task<IEnumerable<ElectricalSafetyTest>> GetFailedTestsAsync()
        {
            return await _unitOfWork.ElectricalSafetyTests.FindAsync(e => e.OverallResult == SafetyTestResult.Fail);
        }

        public async Task<int> GetOverdueCountAsync()
        {
            var overdue = await GetOverdueTestsAsync();
            return overdue.Count();
        }

        public async Task<int> GetPassCountAsync()
        {
            var records = await _unitOfWork.ElectricalSafetyTests.FindAsync(e =>
                e.OverallResult == SafetyTestResult.Pass || e.OverallResult == SafetyTestResult.PassWithRemarks);
            return records.Count();
        }

        public async Task<int> GetFailCountAsync()
        {
            var records = await _unitOfWork.ElectricalSafetyTests.FindAsync(e => e.OverallResult == SafetyTestResult.Fail);
            return records.Count();
        }
    }
}

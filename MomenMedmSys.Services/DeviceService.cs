using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    public interface IDeviceService
    {
        Task<IEnumerable<MedicalDevice>> GetAllDevicesAsync();
        Task<MedicalDevice?> GetDeviceByIdAsync(int id);
        Task<MedicalDevice> CreateDeviceAsync(MedicalDevice device);
        Task UpdateDeviceAsync(MedicalDevice device);
        Task DeleteDeviceAsync(int id);
        Task<IEnumerable<MedicalDevice>> GetDevicesByDepartmentAsync(int departmentId);
        Task<IEnumerable<MedicalDevice>> GetDevicesByStatusAsync(DeviceStatus status);
        Task<IEnumerable<MedicalDevice>> GetDevicesByRiskClassAsync(RiskClass riskClass);
        Task<IEnumerable<MedicalDevice>> GetDevicesDueForMaintenanceAsync();
        Task<IEnumerable<MedicalDevice>> GetDevicesDueForCalibrationAsync();
        Task<IEnumerable<MedicalDevice>> GetDevicesWithExpiringWarrantyAsync(int daysThreshold = 30);
        Task<int> GetTotalDeviceCountAsync();
        Task<int> GetActiveDeviceCountAsync();
        Task<decimal> GetTotalAssetValueAsync();
        Task<decimal> GetDeviceTotalCostAsync(int deviceId);
    }

    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MedicalDevice>> GetAllDevicesAsync()
        {
            return await _unitOfWork.MedicalDevices.GetAllAsync();
        }

        public async Task<MedicalDevice?> GetDeviceByIdAsync(int id)
        {
            return await _unitOfWork.MedicalDevices.GetByIdAsync(id);
        }

        public async Task<MedicalDevice> CreateDeviceAsync(MedicalDevice device)
        {
            await _unitOfWork.MedicalDevices.AddAsync(device);
            return device;
        }

        public async Task UpdateDeviceAsync(MedicalDevice device)
        {
            device.UpdatedAt = DateTime.Now;
            _unitOfWork.MedicalDevices.Update(device);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _unitOfWork.MedicalDevices.GetByIdAsync(id);
            if (device != null)
            {
                _unitOfWork.MedicalDevices.Remove(device);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MedicalDevice>> GetDevicesByDepartmentAsync(int departmentId)
        {
            return await _unitOfWork.MedicalDevices.FindAsync(d => d.DepartmentId == departmentId && d.IsActive);
        }

        public async Task<IEnumerable<MedicalDevice>> GetDevicesByStatusAsync(DeviceStatus status)
        {
            return await _unitOfWork.MedicalDevices.FindAsync(d => d.Status == status);
        }

        public async Task<IEnumerable<MedicalDevice>> GetDevicesByRiskClassAsync(RiskClass riskClass)
        {
            return await _unitOfWork.MedicalDevices.FindAsync(d => d.RiskClassification == riskClass && d.IsActive);
        }

        public async Task<IEnumerable<MedicalDevice>> GetDevicesDueForMaintenanceAsync()
        {
            var allDevices = await _unitOfWork.MedicalDevices.FindAsync(d => d.RequiresPreventiveMaintenance && d.Status == DeviceStatus.Active);
            var now = DateTime.Now;
            return allDevices.Where(d => !d.LastMaintenanceDate.HasValue || d.LastMaintenanceDate.Value.AddDays(30) <= now);
        }

        public async Task<IEnumerable<MedicalDevice>> GetDevicesDueForCalibrationAsync()
        {
            var allDevices = await _unitOfWork.MedicalDevices.FindAsync(d => d.RequiresCalibration && d.Status == DeviceStatus.Active);
            var now = DateTime.Now;
            return allDevices.Where(d => !d.LastCalibrationDate.HasValue || d.LastCalibrationDate.Value.AddDays(90) <= now);
        }

        public async Task<IEnumerable<MedicalDevice>> GetDevicesWithExpiringWarrantyAsync(int daysThreshold = 30)
        {
            var threshold = DateTime.Now.AddDays(daysThreshold);
            return await _unitOfWork.MedicalDevices.FindAsync(d =>
                d.WarrantyExpiryDate <= threshold &&
                d.WarrantyExpiryDate >= DateTime.Now &&
                d.IsActive);
        }

        public async Task<int> GetTotalDeviceCountAsync()
        {
            return await _unitOfWork.MedicalDevices.CountAsync();
        }

        public async Task<int> GetActiveDeviceCountAsync()
        {
            return (await _unitOfWork.MedicalDevices.FindAsync(d => d.IsActive && d.Status == DeviceStatus.Active)).Count();
        }

        public async Task<decimal> GetTotalAssetValueAsync()
        {
            var devices = await _unitOfWork.MedicalDevices.GetAllAsync();
            return devices.Where(d => d.IsActive).Sum(d => d.PurchasePrice);
        }

        public async Task<decimal> GetDeviceTotalCostAsync(int deviceId)
        {
            var device = await _unitOfWork.MedicalDevices.GetByIdAsync(deviceId);
            if (device == null) return 0;

            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.FindAsync(m => m.DeviceId == deviceId);
            return device.PurchasePrice + maintenanceRecords.Sum(m => m.TotalCost);
        }
    }
}

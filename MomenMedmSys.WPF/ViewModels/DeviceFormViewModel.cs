using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class DeviceFormViewModel : ViewModelBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;

        // Mode: "Add" or "Edit"
        public string Mode { get; private set; } = "Add";
        public MedicalDevice? EditingDevice { get; private set; }

        public DeviceFormViewModel(IDeviceService deviceService, IDialogService dialogService)
        {
            _deviceService = deviceService;
            _dialogService = dialogService;
            Title = "Add Device";
        }

        /// <summary>
        /// Initialize for editing an existing device
        /// </summary>
        public void SetEditMode(MedicalDevice device)
        {
            Mode = "Edit";
            EditingDevice = device;
            Title = "Edit Device";

            DeviceCode = device.DeviceCode;
            DeviceName = device.DeviceName;
            Description = device.Description;
            Manufacturer = device.Manufacturer;
            Model = device.Model;
            SerialNumber = device.SerialNumber;
            Category = device.Category;
            Department = device.Department;
            PurchaseDate = device.PurchaseDate == default ? DateTime.Now : device.PurchaseDate;
            PurchasePrice = device.PurchasePrice;
            RiskClassification = device.RiskClassification;
            RequiresCalibration = device.RequiresCalibration;
            RequiresPreventiveMaintenance = device.RequiresPreventiveMaintenance;
            TechnicalSpecs = device.TechnicalSpecs;
            StatusMessage = $"Editing: {device.DeviceCode}";
        }

        /// <summary>
        /// Initialize for adding a new device
        /// </summary>
        public void SetAddMode()
        {
            Mode = "Add";
            EditingDevice = null;
            Title = "Add Device";
            DeviceCode = "DEV-NEW";
            DeviceName = string.Empty;
            Description = string.Empty;
            Manufacturer = string.Empty;
            Model = string.Empty;
            SerialNumber = string.Empty;
            Category = string.Empty;
            Department = string.Empty;
            PurchaseDate = DateTime.Now;
            PurchasePrice = 0;
            RiskClassification = RiskClass.Medium;
            RequiresCalibration = true;
            RequiresPreventiveMaintenance = true;
            TechnicalSpecs = string.Empty;
            StatusMessage = "Fill in device details";
        }

        // Properties
        [ObservableProperty] private string _deviceCode = "DEV-NEW";
        [ObservableProperty] private string _deviceName = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _manufacturer = string.Empty;
        [ObservableProperty] private string _model = string.Empty;
        [ObservableProperty] private string _serialNumber = string.Empty;
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private string _department = string.Empty;
        [ObservableProperty] private DateTime _purchaseDate = DateTime.Now;
        [ObservableProperty] private decimal _purchasePrice;
        [ObservableProperty] private RiskClass _riskClassification = RiskClass.Medium;
        [ObservableProperty] private bool _requiresCalibration = true;
        [ObservableProperty] private bool _requiresPreventiveMaintenance = true;
        [ObservableProperty] private string _technicalSpecs = string.Empty;

        // Department options
        public string[] DepartmentOptions => new[] { "General", "Radiology", "ICU", "Oncology", "Laboratory", "Emergency", "Operating Room", "Obstetrics", "Cardiology", "Pediatrics" };

        // Category options
        public string[] CategoryOptions => new[] { "Imaging", "Monitoring", "Therapeutic", "Laboratory", "Diagnostic", "Surgical", "Respiratory", "Cardiac" };

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(DeviceName))
            {
                await _dialogService.ShowMessageAsync("Device Name is required.", "Validation Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(Manufacturer))
            {
                await _dialogService.ShowMessageAsync("Manufacturer is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingDevice != null)
                {
                    // Update existing
                    EditingDevice.DeviceName = DeviceName;
                    EditingDevice.Description = Description;
                    EditingDevice.Manufacturer = Manufacturer;
                    EditingDevice.Model = Model;
                    EditingDevice.SerialNumber = SerialNumber;
                    EditingDevice.Category = Category;
                    EditingDevice.Department = Department;
                    EditingDevice.PurchaseDate = PurchaseDate;
                    EditingDevice.PurchasePrice = PurchasePrice;
                    EditingDevice.RiskClassification = RiskClassification;
                    EditingDevice.RequiresCalibration = RequiresCalibration;
                    EditingDevice.RequiresPreventiveMaintenance = RequiresPreventiveMaintenance;
                    EditingDevice.TechnicalSpecs = TechnicalSpecs;
                    EditingDevice.UpdatedAt = DateTime.Now;

                    await _deviceService.UpdateDeviceAsync(EditingDevice);
                    StatusMessage = $"Updated: {DeviceName}";
                }
                else
                {
                    // Create new
                    var device = new MedicalDevice
                    {
                        DeviceCode = string.IsNullOrWhiteSpace(DeviceCode) ? $"DEV-{DateTime.Now:yyyyMMdd}" : DeviceCode,
                        DeviceName = DeviceName,
                        Description = Description,
                        Manufacturer = Manufacturer,
                        Model = Model,
                        SerialNumber = SerialNumber,
                        Category = Category,
                        Department = Department,
                        PurchaseDate = PurchaseDate,
                        PurchasePrice = PurchasePrice,
                        RiskClassification = RiskClassification,
                        RequiresCalibration = RequiresCalibration,
                        RequiresPreventiveMaintenance = RequiresPreventiveMaintenance,
                        TechnicalSpecs = TechnicalSpecs,
                        Status = DeviceStatus.Active,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _deviceService.CreateDeviceAsync(device);
                    StatusMessage = $"Created: {DeviceName}";
                }

                // Navigate back
                App.MainViewModelInstance?.GoBackCommand.Execute(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save error: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to save device: {ex.Message}", "Error");
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            StatusMessage = "Form cancelled";
            App.MainViewModelInstance?.GoBackCommand.Execute(null);
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class DeviceListViewModel : ViewModelBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;
        private readonly Func<DeviceFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public DeviceListViewModel(IDeviceService deviceService, IDialogService dialogService,
            IExportService exportService, Func<DeviceFormViewModel> formFactory)
        {
            _deviceService = deviceService;
            _dialogService = dialogService;
            _exportService = exportService;
            _formFactory = formFactory;
            Title = "Device Register";
            LoadDevicesCommand.Execute(null);
        }

        /// <summary>
        /// Set MainViewModel reference after construction to avoid circular DI
        /// </summary>
        public void SetMainViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
        }

        public ObservableCollection<MedicalDevice> Devices { get; } = new();
        public ObservableCollection<MedicalDevice> FilteredDevices { get; } = new();

        // Summary stats
        [ObservableProperty] private int _activeCount;
        [ObservableProperty] private int _maintenanceCount;
        [ObservableProperty] private int _criticalCount;
        [ObservableProperty] private string _totalAssetValue = "$0";

        private MedicalDevice? _selectedDevice;
        public MedicalDevice? SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        private string _filterStatus = "All";
        public string FilterStatus
        {
            get => _filterStatus;
            set
            {
                if (SetProperty(ref _filterStatus, value))
                {
                    ApplyFilter();
                }
            }
        }

        private string _filterDepartment = "All";
        public string FilterDepartment
        {
            get => _filterDepartment;
            set
            {
                if (SetProperty(ref _filterDepartment, value))
                {
                    ApplyFilter();
                }
            }
        }

        [RelayCommand]
        private async Task LoadDevices()
        {
            IsLoading = true;
            try
            {
                Devices.Clear();
                var allDevices = await _deviceService.GetAllDevicesAsync();
                foreach (var device in allDevices)
                    Devices.Add(device);
                ApplyFilter();
                UpdateStats();
                StatusMessage = $"Loaded {Devices.Count} devices";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading devices: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateStats()
        {
            ActiveCount = Devices.Count(d => d.Status == DeviceStatus.Active);
            MaintenanceCount = Devices.Count(d => d.Status == DeviceStatus.UnderMaintenance);
            CriticalCount = Devices.Count(d => d.RiskClassification == RiskClass.Critical);
            var total = Devices.Sum(d => d.PurchasePrice);
            TotalAssetValue = total >= 1000000
                ? $"${total / 1000000:F2}M"
                : $"${total / 1000:F1}K";
        }

        private void ApplyFilter()
        {
            FilteredDevices.Clear();
            var query = Devices.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(d =>
                    d.DeviceName.ToLower().Contains(search) ||
                    d.DeviceCode.ToLower().Contains(search) ||
                    d.Manufacturer.ToLower().Contains(search) ||
                    d.Model.ToLower().Contains(search) ||
                    d.Department.ToLower().Contains(search) ||
                    d.SerialNumber.ToLower().Contains(search));
            }

            if (FilterStatus != "All" && FilterStatus != "All Statuses")
            {
                if (Enum.TryParse<DeviceStatus>(FilterStatus, out var status))
                {
                    query = query.Where(d => d.Status == status);
                }
            }

            if (FilterDepartment != "All" && FilterDepartment != "All Departments")
            {
                query = query.Where(d => d.Department == FilterDepartment);
            }

            foreach (var device in query)
                FilteredDevices.Add(device);
        }

        private void OnFormSaved()
        {
            LoadDevicesCommand.Execute(null);
        }

        [RelayCommand]
        private void AddDevice()
        {
            var form = _formFactory();
            form.SetAddMode();
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task EditDevice()
        {
            if (SelectedDevice == null)
            {
                await _dialogService.ShowMessageAsync("Please select a device to edit.", "No Selection");
                return;
            }

            var form = _formFactory();
            form.SetEditMode(SelectedDevice);
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task DeleteDevice()
        {
            if (SelectedDevice == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Are you sure you want to delete '{SelectedDevice.DeviceName}'?", "Confirm Delete");

            if (confirmed)
            {
                try
                {
                    await _deviceService.DeleteDeviceAsync(SelectedDevice.Id);
                    Devices.Remove(SelectedDevice);
                    ApplyFilter();
                    UpdateStats();
                    SelectedDevice = null;
                    StatusMessage = "Device deleted";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error deleting device: {ex.Message}";
                }
            }
        }

        [RelayCommand]
        private async Task ExportCsv()
        {
            if (FilteredDevices.Count == 0)
            {
                await _dialogService.ShowMessageAsync("No devices to export.", "Export");
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV Files|*.csv",
                    FileName = $"Devices_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    DefaultExt = "csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    using var writer = new StreamWriter(saveDialog.FileName);
                    writer.WriteLine("DeviceCode,DeviceName,Manufacturer,Model,SerialNumber,Category,Department,Building,Floor,Room,PurchaseDate,PurchasePrice,Supplier,WarrantyExpiry,Status,RiskClass");

                    foreach (var d in FilteredDevices)
                    {
                        writer.WriteLine(
                            $"\"{d.DeviceCode}\",\"{d.DeviceName}\",\"{d.Manufacturer}\",\"{d.Model}\"," +
                            $"\"{d.SerialNumber}\",\"{d.Category}\",\"{d.Department}\",\"{d.Building}\"," +
                            $"\"{d.Floor}\",\"{d.Room}\",{d.PurchaseDate:yyyy-MM-dd},{d.PurchasePrice}," +
                            $"\"{d.SupplierName}\",{d.WarrantyExpiryDate:yyyy-MM-dd},\"{d.Status}\",\"{d.RiskClassification}\"");
                    }

                    StatusMessage = $"✅ Exported {FilteredDevices.Count} devices to {saveDialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Export failed: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task PrintReport()
        {
            if (FilteredDevices.Count == 0)
            {
                await _dialogService.ShowMessageAsync("No devices to export.", "Export");
                return;
            }

            // Show format selection dialog
            var result = await _dialogService.ShowConfirmAsync(
                "Select export format:\n\n• PDF\n• Excel (XLSX)\n• CSV\n• Word (TXT)\n\nClick OK for PDF, Cancel for Excel.",
                "Export Format");

            string filePath = string.Empty;
            string ext = string.Empty;

            // Determine format and file path
            if (result) // OK → PDF
            {
                ext = "pdf";
                var dialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    DefaultExt = ".pdf",
                    FileName = $"Devices_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Export as PDF"
                };
                if (dialog.ShowDialog() != true) return;
                filePath = dialog.FileName;
            }
            else // Cancel → Ask for Excel vs CSV vs Word
            {
                var result2 = await _dialogService.ShowConfirmAsync(
                    "Click OK for Excel (XLSX), Cancel for CSV.",
                    "Excel or CSV?");

                if (result2) // Excel
                {
                    ext = "xlsx";
                    var dialog = new SaveFileDialog
                    {
                        Filter = "Excel Files|*.xlsx",
                        DefaultExt = ".xlsx",
                        FileName = $"Devices_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                        Title = "Export as Excel"
                    };
                    if (dialog.ShowDialog() != true) return;
                    filePath = dialog.FileName;
                }
                else // CSV or Word
                {
                    var result3 = await _dialogService.ShowConfirmAsync(
                        "Click OK for CSV, Cancel for Word/Text.",
                        "CSV or Word?");

                    if (result3) // CSV
                    {
                        ext = "csv";
                        var dialog = new SaveFileDialog
                        {
                            Filter = "CSV Files|*.csv",
                            DefaultExt = ".csv",
                            FileName = $"Devices_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                            Title = "Export as CSV"
                        };
                        if (dialog.ShowDialog() != true) return;
                        filePath = dialog.FileName;
                    }
                    else // Word/TXT
                    {
                        ext = "txt";
                        var dialog = new SaveFileDialog
                        {
                            Filter = "Text Files (*.txt)|*.txt|Word Documents (*.doc)|*.doc",
                            DefaultExt = ".txt",
                            FileName = $"Devices_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                            Title = "Export as Text/Word"
                        };
                        if (dialog.ShowDialog() != true) return;
                        filePath = dialog.FileName;
                    }
                }
            }

            try
            {
                var headers = new[] { "Code", "Device Name", "Manufacturer", "Model", "Serial", "Category", "Department", "Building", "Room", "Purchase Date", "Price", "Supplier", "Warranty", "Status", "Risk" };
                var rows = new List<object[]>();
                foreach (var d in FilteredDevices)
                {
                    rows.Add(new object[]
                    {
                        d.DeviceCode, d.DeviceName, d.Manufacturer, d.Model,
                        d.SerialNumber, d.Category, d.Department, d.Building,
                        d.Room, d.PurchaseDate.ToString("yyyy-MM-dd"), d.PurchasePrice,
                        d.SupplierName, d.WarrantyExpiryDate.ToString("yyyy-MM-dd"),
                        d.Status.ToString(), d.RiskClassification.ToString()
                    });
                }

                switch (ext.ToLower())
                {
                    case "pdf":
                        await _exportService.ExportToPdfAsync(
                            filePath,
                            "Medical Equipment Device Register",
                            $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}  |  Total Devices: {FilteredDevices.Count}",
                            headers, rows);
                        break;

                    case "xlsx":
                        await _exportService.ExportToExcelAsync(
                            filePath, "Devices", headers, rows);
                        break;

                    case "csv":
                        await _exportService.ExportToCsvAsync(
                            filePath, headers, rows);
                        break;

                    default: // txt/doc
                        await ExportAsText(filePath);
                        break;
                }

                StatusMessage = $"📄 Report exported: {System.IO.Path.GetFileName(filePath)}";

                if (await _dialogService.ShowConfirmAsync($"Report saved to:\n{filePath}\n\nOpen for printing?", "Open Report"))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = filePath, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Export error: {ex.Message}";
            }
        }

        private async Task ExportAsText(string filePath)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════════");
            sb.AppendLine("   MEDICAL EQUIPMENT DEVICE REGISTER");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Total Devices: {FilteredDevices.Count}");
            sb.AppendLine();
            sb.AppendLine("───────────────────────────────────────────────────────────────────");
            sb.AppendLine("  DEVICE LISTING");
            sb.AppendLine("───────────────────────────────────────────────────────────────────");
            sb.AppendLine($"{"Code",-14} {"Device Name",-25} {"Manufacturer",-18} {"Department",-18} {"Status",-16} {"Risk",-8}");
            sb.AppendLine(new string('-', 100));

            foreach (var d in FilteredDevices)
            {
                sb.AppendLine($"{d.DeviceCode,-14} {d.DeviceName,-25} {d.Manufacturer,-18} {d.Department,-18} {d.Status,-16} {d.RiskClassification,-8}");
            }

            sb.AppendLine();
            sb.AppendLine("═══════════════════════════════════════════════════════════════════");
            sb.AppendLine("  End of Report");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════");

            await System.IO.File.WriteAllTextAsync(filePath, sb.ToString());
        }
    }
}

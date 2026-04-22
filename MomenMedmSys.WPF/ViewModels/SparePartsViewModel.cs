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
    public partial class SparePartsViewModel : ViewModelBase
    {
        private readonly ISparePartService _sparePartService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly IExportService _exportService;
        private readonly Func<SparePartFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public SparePartsViewModel(ISparePartService sparePartService, IDeviceService deviceService,
            IDialogService dialogService, IExportService exportService, Func<SparePartFormViewModel> formFactory)
        {
            _sparePartService = sparePartService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _exportService = exportService;
            _formFactory = formFactory;
            Title = "Spare Parts";
            LoadPartsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<SparePart> Parts { get; } = new();
        public ObservableCollection<SparePart> FilteredParts { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private SparePart? _selectedPart;
        [ObservableProperty] private int _lowStockCount;
        [ObservableProperty] private int _criticalCount;
        [ObservableProperty] private string _totalInventoryValue = "$0";
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private string _filterCategory = "All";

        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnFilterCategoryChanged(string value) => ApplyFilter();

        [RelayCommand]
        private async Task LoadParts()
        {
            IsLoading = true;
            try
            {
                Parts.Clear();
                var all = await _sparePartService.GetAllPartsAsync();
                foreach (var p in all) Parts.Add(p);

                Devices.Clear();
                var devices = await _deviceService.GetAllDevicesAsync();
                foreach (var d in devices) Devices.Add(d);

                LowStockCount = await _sparePartService.GetLowStockCountAsync();
                var totalVal = await _sparePartService.GetTotalInventoryValueAsync();
                TotalInventoryValue = totalVal >= 1000000 ? $"${totalVal / 1000000:F2}M" : $"${totalVal / 1000:F1}K";
                CriticalCount = Parts.Count(p => p.IsCritical && p.CurrentStock <= p.MinimumStock);

                ApplyFilter();
                StatusMessage = $"Loaded {Parts.Count} spare parts";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private void ApplyFilter()
        {
            FilteredParts.Clear();
            var query = Parts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var s = SearchText.ToLower();
                query = query.Where(p => p.PartName.ToLower().Contains(s) || p.PartNumber.ToLower().Contains(s) ||
                    (p.Category ?? "").ToLower().Contains(s) || (p.StorageLocation ?? "").ToLower().Contains(s));
            }

            if (FilterCategory != "All" && FilterCategory != "All Categories")
            {
                query = query.Where(p => p.Category == FilterCategory);
            }

            foreach (var p in query) FilteredParts.Add(p);
        }

        [RelayCommand]
        private async Task DeletePart()
        {
            if (SelectedPart == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync("Delete this spare part?", "Confirm");
            if (confirmed)
            {
                await _sparePartService.DeletePartAsync(SelectedPart.Id);
                Parts.Remove(SelectedPart);
                ApplyFilter();
                SelectedPart = null;
                StatusMessage = "Part deleted";
            }
        }

        [RelayCommand]
        private void AddPart()
        {
            var form = _formFactory();
            form.SetAddMode();
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task EditPart()
        {
            if (SelectedPart == null) { await _dialogService.ShowMessageAsync("Select a part to edit.", "No Selection"); return; }
            var form = _formFactory();
            form.SetEditMode(SelectedPart);
            _mainVM?.NavigateTo(form);
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            var dialog = new SaveFileDialog { Filter = "Excel Files (*.xlsx)|*.xlsx", DefaultExt = ".xlsx", FileName = $"SpareParts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx", Title = "Export to Excel" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Part#", "Name", "Category", "Stock", "Min", "Unit Cost", "Total Value", "Location", "Device" };
                var rows = FilteredParts.Select(p => new object[] {
                    p.PartNumber, p.PartName, p.Category, p.CurrentStock, p.MinimumStock,
                    p.UnitCost, p.TotalUsageValue, p.StorageLocation, p.Device?.DeviceName ?? ""
                });
                await _exportService.ExportToExcelAsync(dialog.FileName, "Spare Parts Inventory", headers, rows);
                StatusMessage = $"📊 Exported to Excel: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task ExportToCsv()
        {
            var dialog = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", DefaultExt = ".csv", FileName = $"SpareParts_{DateTime.Now:yyyyMMdd_HHmmss}.csv", Title = "Export to CSV" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Part#", "Name", "Category", "Stock", "Min", "Unit Cost", "Total Value", "Location", "Device" };
                var rows = FilteredParts.Select(p => new object[] {
                    p.PartNumber, p.PartName, p.Category, p.CurrentStock, p.MinimumStock,
                    p.UnitCost, p.TotalUsageValue, p.StorageLocation, p.Device?.DeviceName ?? ""
                });
                await _exportService.ExportToCsvAsync(dialog.FileName, headers, rows);
                StatusMessage = $"📄 Exported to CSV: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        [RelayCommand]
        private async Task ExportToPdf()
        {
            var dialog = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", DefaultExt = ".pdf", FileName = $"SpareParts_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", Title = "Export to PDF" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                var headers = new[] { "Part#", "Name", "Category", "Stock", "Min", "Cost", "Location" };
                var rows = FilteredParts.Select(p => new object[] {
                    p.PartNumber, p.PartName, p.Category, p.CurrentStock, p.MinimumStock,
                    p.UnitCost.ToString("C2"), p.StorageLocation
                });
                var summary = new (string, string)[] {
                    ("Total Parts", FilteredParts.Count.ToString()),
                    ("Inventory Value", TotalInventoryValue),
                    ("Low Stock", LowStockCount.ToString()),
                    ("Critical", CriticalCount.ToString())
                };
                await _exportService.ExportToPdfAsync(dialog.FileName, "Spare Parts Inventory Report",
                    $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", headers, rows, summary);
                StatusMessage = $"📑 Exported to PDF: {Path.GetFileName(dialog.FileName)}";
                OpenFile(dialog.FileName);
            }
            catch (Exception ex) { StatusMessage = $"❌ Export error: {ex.Message}"; }
        }

        private async void OpenFile(string path)
        {
            if (await _dialogService.ShowConfirmAsync($"File saved:\n{path}\n\nOpen file?", "Open File"))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = path, UseShellExecute = true });
        }
    }
}

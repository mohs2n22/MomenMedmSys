using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly Func<SparePartFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public SparePartsViewModel(ISparePartService sparePartService, IDeviceService deviceService,
            IDialogService dialogService, Func<SparePartFormViewModel> formFactory)
        {
            _sparePartService = sparePartService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _formFactory = formFactory;
            Title = "Spare Parts";
            LoadPartsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<SparePart> Parts { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private SparePart? _selectedPart;
        [ObservableProperty] private int _lowStockCount;
        [ObservableProperty] private decimal _totalInventoryValue;
        [ObservableProperty] private int _totalCount;

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
                TotalInventoryValue = await _sparePartService.GetTotalInventoryValueAsync();
                TotalCount = Parts.Count;

                StatusMessage = $"Loaded {TotalCount} spare parts";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
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
                SelectedPart = null;
                TotalCount = Parts.Count;
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
        private void EditPart()
        {
            if (SelectedPart == null) { _dialogService.ShowMessageAsync("Select a part to edit.", "No Selection"); return; }
            var form = _formFactory();
            form.SetEditMode(SelectedPart);
            _mainVM?.NavigateTo(form);
        }
    }
}

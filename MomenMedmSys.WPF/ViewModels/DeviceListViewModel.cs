using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    public partial class DeviceListViewModel : ViewModelBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly Func<DeviceFormViewModel> _formFactory;
        private MainViewModel? _mainVM;

        public DeviceListViewModel(IDeviceService deviceService, IDialogService dialogService,
            Func<DeviceFormViewModel> formFactory)
        {
            _deviceService = deviceService;
            _dialogService = dialogService;
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
                    d.Department.ToLower().Contains(search));
            }

            if (FilterStatus != "All")
            {
                if (Enum.TryParse<DeviceStatus>(FilterStatus, out var status))
                {
                    query = query.Where(d => d.Status == status);
                }
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
        private void EditDevice()
        {
            if (SelectedDevice == null)
            {
                _dialogService.ShowMessageAsync("Please select a device to edit.", "No Selection");
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
                    SelectedDevice = null;
                    StatusMessage = "Device deleted";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error deleting device: {ex.Message}";
                }
            }
        }
    }
}

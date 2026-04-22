using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class ProcurementViewModel : ViewModelBase
    {
        private readonly IProcurementService _procurementService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly Func<ProcurementFormViewModel> _formViewModelFactory;
        private MainViewModel? _mainVM;

        public ProcurementViewModel(IProcurementService procurementService, IDeviceService deviceService,
            IDialogService dialogService, Func<ProcurementFormViewModel> formViewModelFactory)
        {
            _procurementService = procurementService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _formViewModelFactory = formViewModelFactory;
            Title = "Procurement Requests";
        }

        public async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                var requests = await _procurementService.GetAllAsync();
                ProcurementRequests.Clear();
                foreach (var pr in requests.OrderByDescending(p => p.RequestDate))
                    ProcurementRequests.Add(pr);

                TotalCount = ProcurementRequests.Count;
                PendingCount = await _procurementService.GetPendingCountAsync();
                ApprovedCount = await _procurementService.GetApprovedCountAsync();
                StatusMessage = $"Loaded {TotalCount} procurement requests";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading procurement requests: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public ObservableCollection<ProcurementRequest> ProcurementRequests { get; } = new();
        public int TotalCount { get; private set; }
        public int PendingCount { get; private set; }
        public int ApprovedCount { get; private set; }

        private ProcurementStatus _filterStatus = ProcurementStatus.Draft;
        public ProcurementStatus FilterStatus
        {
            get => _filterStatus;
            set
            {
                SetProperty(ref _filterStatus, value);
                FilterRequestsCommand.Execute(null);
            }
        }

        public ProcurementStatus[] StatusOptions => (ProcurementStatus[])Enum.GetValues(typeof(ProcurementStatus));

        [RelayCommand]
        private void FilterRequests()
        {
            ProcurementRequests.Clear();
            // Simplified - would reload with filter in production
        }

        [RelayCommand]
        private async Task AddRequest()
        {
            var formVm = _formViewModelFactory();
            formVm.SetAddMode();
            _mainVM?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task EditRequest(ProcurementRequest? request)
        {
            if (request == null) return;
            var formVm = _formViewModelFactory();
            formVm.SetEditMode(request);
            _mainVM?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteRequest(ProcurementRequest? request)
        {
            if (request == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Are you sure you want to delete request '{request.RequestNumber}'?", "Confirm Delete");
            if (confirmed)
            {
                await _procurementService.DeleteAsync(request.Id);
                StatusMessage = $"Deleted request: {request.RequestNumber}";
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task Refresh() => await LoadDataAsync();

        public void SetMainViewModel(MainViewModel mainVM) => _mainVM = mainVM;
    }
}

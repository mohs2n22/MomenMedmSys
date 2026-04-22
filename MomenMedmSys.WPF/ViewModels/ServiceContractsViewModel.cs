using System;
using System.Collections.ObjectModel;
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
    public partial class ServiceContractsViewModel : ViewModelBase
    {
        private readonly IServiceContractService _contractService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly Func<ServiceContractFormViewModel> _formViewModelFactory;

        public ServiceContractsViewModel(IServiceContractService contractService, IDeviceService deviceService,
            IDialogService dialogService, Func<ServiceContractFormViewModel> formViewModelFactory)
        {
            _contractService = contractService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _formViewModelFactory = formViewModelFactory;
            Title = "Service Contracts";
        }

        public async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                var contracts = await _contractService.GetAllContractsAsync();
                Contracts.Clear();
                foreach (var c in contracts)
                    Contracts.Add(c);

                TotalCount = Contracts.Count;
                ActiveCount = await _contractService.GetActiveContractCountAsync();
                ExpiringCount = await _contractService.GetExpiringSoonCountAsync();
                StatusMessage = $"Loaded {TotalCount} service contracts";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading contracts: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public ObservableCollection<ServiceContract> Contracts { get; } = new();
        public int TotalCount { get; private set; }
        public int ActiveCount { get; private set; }
        public int ExpiringCount { get; private set; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterContractsCommand.Execute(null);
            }
        }

        [RelayCommand]
        private void FilterContracts()
        {
            Contracts.Clear();
            // Reload and filter would go here - simplified for brevity
        }

        [RelayCommand]
        private async Task AddContract()
        {
            var formVm = _formViewModelFactory();
            formVm.SetAddMode();
            App.MainViewModelInstance?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task EditContract(ServiceContract? contract)
        {
            if (contract == null) return;
            var formVm = _formViewModelFactory();
            formVm.SetEditMode(contract);
            App.MainViewModelInstance?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteContract(ServiceContract? contract)
        {
            if (contract == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Are you sure you want to delete contract '{contract.ContractName}'?", "Confirm Delete");
            if (confirmed)
            {
                await _contractService.DeleteContractAsync(contract.Id);
                StatusMessage = $"Deleted contract: {contract.ContractName}";
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task Refresh() => await LoadDataAsync();

        public void SetMainViewModel(MainViewModel mainVM) => _mainVM = mainVM;
        private MainViewModel? _mainVM;
    }
}

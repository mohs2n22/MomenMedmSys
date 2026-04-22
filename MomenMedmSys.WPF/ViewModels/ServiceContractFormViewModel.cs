using System;
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
    public partial class ServiceContractFormViewModel : ViewModelBase
    {
        private readonly IServiceContractService _contractService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public ServiceContract? EditingContract { get; private set; }

        public ServiceContractFormViewModel(IServiceContractService contractService, IDialogService dialogService)
        {
            _contractService = contractService;
            _dialogService = dialogService;
            Title = "Add Service Contract";
        }

        public void SetEditMode(ServiceContract contract)
        {
            Mode = "Edit";
            EditingContract = contract;
            Title = "Edit Service Contract";

            ContractNumber = contract.ContractNumber;
            ContractName = contract.ContractName;
            Provider = contract.Provider;
            ContactPerson = contract.ContactPerson;
            ContactEmail = contract.ContactEmail;
            ContactPhone = contract.ContactPhone;
            StartDate = contract.StartDate;
            EndDate = contract.EndDate;
            AutoRenew = contract.AutoRenew;
            RenewalNoticeDays = contract.RenewalNoticeDays;
            CoverageDescription = contract.CoverageDescription;
            ContractValue = contract.ContractValue;
            ResponseTimeHours = contract.ResponseTimeHours;
            ResolutionTimeHours = contract.ResolutionTimeHours;
            Status = contract.Status;
            StatusMessage = $"Editing: {contract.ContractName}";
        }

        public void SetAddMode()
        {
            Mode = "Add";
            EditingContract = null;
            Title = "Add Service Contract";
            ContractNumber = string.Empty;
            ContractName = string.Empty;
            Provider = string.Empty;
            ContactPerson = string.Empty;
            ContactEmail = string.Empty;
            ContactPhone = string.Empty;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddYears(1);
            AutoRenew = false;
            RenewalNoticeDays = 30;
            CoverageDescription = string.Empty;
            ContractValue = 0;
            ResponseTimeHours = 4;
            ResolutionTimeHours = 24;
            Status = ContractStatus.Active;
            StatusMessage = "Fill in contract details";
        }

        [ObservableProperty] private string _contractNumber = string.Empty;
        [ObservableProperty] private string _contractName = string.Empty;
        [ObservableProperty] private string _provider = string.Empty;
        [ObservableProperty] private string _contactPerson = string.Empty;
        [ObservableProperty] private string _contactEmail = string.Empty;
        [ObservableProperty] private string _contactPhone = string.Empty;
        [ObservableProperty] private DateTime _startDate = DateTime.Now;
        [ObservableProperty] private DateTime _endDate = DateTime.Now.AddYears(1);
        [ObservableProperty] private bool _autoRenew;
        [ObservableProperty] private int _renewalNoticeDays = 30;
        [ObservableProperty] private string _coverageDescription = string.Empty;
        [ObservableProperty] private decimal _contractValue;
        [ObservableProperty] private int _responseTimeHours;
        [ObservableProperty] private int _resolutionTimeHours;
        [ObservableProperty] private ContractStatus _status = ContractStatus.Active;

        public ContractStatus[] StatusOptions => (ContractStatus[])Enum.GetValues(typeof(ContractStatus));

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(ContractName))
            {
                await _dialogService.ShowMessageAsync("Contract name is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingContract != null)
                {
                    EditingContract.ContractNumber = ContractNumber;
                    EditingContract.ContractName = ContractName;
                    EditingContract.Provider = Provider;
                    EditingContract.ContactPerson = ContactPerson;
                    EditingContract.ContactEmail = ContactEmail;
                    EditingContract.ContactPhone = ContactPhone;
                    EditingContract.StartDate = StartDate;
                    EditingContract.EndDate = EndDate;
                    EditingContract.AutoRenew = AutoRenew;
                    EditingContract.RenewalNoticeDays = RenewalNoticeDays;
                    EditingContract.CoverageDescription = CoverageDescription;
                    EditingContract.ContractValue = ContractValue;
                    EditingContract.ResponseTimeHours = ResponseTimeHours;
                    EditingContract.ResolutionTimeHours = ResolutionTimeHours;
                    EditingContract.Status = Status;
                    EditingContract.UpdatedAt = DateTime.Now;

                    await _contractService.UpdateContractAsync(EditingContract);
                    StatusMessage = $"Updated: {ContractName}";
                }
                else
                {
                    var contract = new ServiceContract
                    {
                        ContractNumber = string.IsNullOrWhiteSpace(ContractNumber) ? $"SC-{DateTime.Now:yyyy}-{DateTime.Now:yyyyMMdd}" : ContractNumber,
                        ContractName = ContractName,
                        Provider = Provider,
                        ContactPerson = ContactPerson,
                        ContactEmail = ContactEmail,
                        ContactPhone = ContactPhone,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        AutoRenew = AutoRenew,
                        RenewalNoticeDays = RenewalNoticeDays,
                        CoverageDescription = CoverageDescription,
                        ContractValue = ContractValue,
                        ResponseTimeHours = ResponseTimeHours,
                        ResolutionTimeHours = ResolutionTimeHours,
                        Status = Status,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _contractService.CreateContractAsync(contract);
                    StatusMessage = $"Created: {ContractName}";
                }

                App.MainViewModelInstance?.GoBackCommand.Execute(null);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save error: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to save: {ex.Message}", "Error");
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

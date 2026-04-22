using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class ProcurementFormViewModel : ViewModelBase
    {
        private readonly IProcurementService _procurementService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public ProcurementRequest? EditingRequest { get; private set; }

        public ProcurementFormViewModel(IProcurementService procurementService, IDialogService dialogService)
        {
            _procurementService = procurementService;
            _dialogService = dialogService;
            Title = "Add Procurement Request";
        }

        public void SetEditMode(ProcurementRequest request)
        {
            Mode = "Edit";
            EditingRequest = request;
            Title = "Edit Procurement Request";

            RequestNumber = request.RequestNumber;
            RequestedBy = request.RequestedBy;
            Department = request.Department;
            Justification = request.Justification;
            EquipmentType = request.EquipmentType;
            TechnicalSpecifications = request.TechnicalSpecifications;
            BudgetEstimate = request.BudgetEstimate;
            BudgetApproved = request.BudgetApproved;
            BudgetSource = request.BudgetSource;
            Status = request.Status;
            ApprovedBy = request.ApprovedBy;
            Notes = request.Notes;
            StatusMessage = $"Editing: {request.RequestNumber}";
        }

        public void SetAddMode()
        {
            Mode = "Add";
            EditingRequest = null;
            Title = "Add Procurement Request";
            RequestNumber = string.Empty;
            RequestedBy = string.Empty;
            RequestDate = DateTime.Now;
            Department = string.Empty;
            Justification = string.Empty;
            EquipmentType = string.Empty;
            TechnicalSpecifications = string.Empty;
            BudgetEstimate = 0;
            BudgetApproved = 0;
            BudgetSource = string.Empty;
            Status = ProcurementStatus.Draft;
            ApprovedBy = string.Empty;
            Notes = string.Empty;
            StatusMessage = "Fill in procurement request details";
        }

        [ObservableProperty] private string _requestNumber = string.Empty;
        [ObservableProperty] private string _requestedBy = string.Empty;
        [ObservableProperty] private DateTime _requestDate = DateTime.Now;
        [ObservableProperty] private string _department = string.Empty;
        [ObservableProperty] private string _justification = string.Empty;
        [ObservableProperty] private string _equipmentType = string.Empty;
        [ObservableProperty] private string _technicalSpecifications = string.Empty;
        [ObservableProperty] private decimal _budgetEstimate;
        [ObservableProperty] private decimal _budgetApproved;
        [ObservableProperty] private string _budgetSource = string.Empty;
        [ObservableProperty] private ProcurementStatus _status = ProcurementStatus.Draft;
        [ObservableProperty] private string _approvedBy = string.Empty;
        [ObservableProperty] private DateTime? _approvalDate;
        [ObservableProperty] private string _notes = string.Empty;

        public ProcurementStatus[] StatusOptions => (ProcurementStatus[])Enum.GetValues(typeof(ProcurementStatus));

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(EquipmentType))
            {
                await _dialogService.ShowMessageAsync("Equipment type is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingRequest != null)
                {
                    EditingRequest.RequestNumber = RequestNumber;
                    EditingRequest.RequestedBy = RequestedBy;
                    EditingRequest.Department = Department;
                    EditingRequest.Justification = Justification;
                    EditingRequest.EquipmentType = EquipmentType;
                    EditingRequest.TechnicalSpecifications = TechnicalSpecifications;
                    EditingRequest.BudgetEstimate = BudgetEstimate;
                    EditingRequest.BudgetApproved = BudgetApproved;
                    EditingRequest.BudgetSource = BudgetSource;
                    EditingRequest.Status = Status;
                    EditingRequest.ApprovedBy = ApprovedBy;
                    EditingRequest.Notes = Notes;
                    EditingRequest.UpdatedAt = DateTime.Now;

                    await _procurementService.UpdateAsync(EditingRequest);
                    StatusMessage = $"Updated: {RequestNumber}";
                }
                else
                {
                    var request = new ProcurementRequest
                    {
                        RequestNumber = string.IsNullOrWhiteSpace(RequestNumber) ? $"PR-{DateTime.Now:yyyy}-{DateTime.Now:yyyyMMdd}" : RequestNumber,
                        RequestedBy = RequestedBy,
                        RequestDate = RequestDate,
                        Department = Department,
                        Justification = Justification,
                        EquipmentType = EquipmentType,
                        TechnicalSpecifications = TechnicalSpecifications,
                        BudgetEstimate = BudgetEstimate,
                        BudgetApproved = BudgetApproved,
                        BudgetSource = BudgetSource,
                        Status = Status,
                        ApprovedBy = ApprovedBy,
                        ApprovalDate = ApprovalDate,
                        Notes = Notes,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _procurementService.CreateAsync(request);
                    StatusMessage = $"Created: {request.RequestNumber}";
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

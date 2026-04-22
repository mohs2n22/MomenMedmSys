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
    public partial class WorkOrderFormViewModel : ViewModelBase
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public WorkOrder? EditingWorkOrder { get; private set; }

        public WorkOrderFormViewModel(IWorkOrderService workOrderService, IDeviceService deviceService, IDialogService dialogService)
        {
            _workOrderService = workOrderService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            Title = "Add Work Order";
        }

        public void SetEditMode(WorkOrder workOrder)
        {
            Mode = "Edit";
            EditingWorkOrder = workOrder;
            Title = "Edit Work Order";

            DeviceId = workOrder.DeviceId;
            ReportedBy = workOrder.ReportedBy;
            FaultDescription = workOrder.FaultDescription;
            Priority = workOrder.Priority;
            DeviceCategory = workOrder.DeviceCategory;
            AssignedTo = workOrder.AssignedTo;
            IsExternalContractor = workOrder.IsExternalContractor;
            ContractorName = workOrder.ContractorName;
            ScheduledStartDate = workOrder.ScheduledStartDate ?? DateTime.Now;
            ScheduledEndDate = workOrder.ScheduledEndDate ?? DateTime.Now.AddDays(1);
            Status = workOrder.Status;
            ResolutionDescription = workOrder.ResolutionDescription;
            RootCause = workOrder.RootCause;
            EstimatedCost = workOrder.EstimatedCost;
            ActualCost = workOrder.ActualCost;
            Notes = workOrder.Notes;
            StatusMessage = $"Editing: {workOrder.WorkOrderNumber}";
        }

        public void SetAddMode(int? defaultDeviceId = null)
        {
            Mode = "Add";
            EditingWorkOrder = null;
            Title = "Add Work Order";
            DeviceId = defaultDeviceId ?? 0;
            ReportedBy = string.Empty;
            FaultDescription = string.Empty;
            Priority = WorkOrderPriority.Medium;
            DeviceCategory = string.Empty;
            AssignedTo = string.Empty;
            IsExternalContractor = false;
            ContractorName = string.Empty;
            ScheduledStartDate = DateTime.Now;
            ScheduledEndDate = DateTime.Now.AddDays(1);
            Status = WorkOrderStatus.Open;
            ResolutionDescription = string.Empty;
            RootCause = string.Empty;
            EstimatedCost = 0;
            ActualCost = 0;
            Notes = string.Empty;
            StatusMessage = "Fill in work order details";
        }

        [ObservableProperty] private int _deviceId;
        [ObservableProperty] private string _reportedBy = string.Empty;
        [ObservableProperty] private DateTime _reportDate = DateTime.Now;
        [ObservableProperty] private string _faultDescription = string.Empty;
        [ObservableProperty] private WorkOrderPriority _priority = WorkOrderPriority.Medium;
        [ObservableProperty] private string _deviceCategory = string.Empty;
        [ObservableProperty] private string _assignedTo = string.Empty;
        [ObservableProperty] private bool _isExternalContractor;
        [ObservableProperty] private string _contractorName = string.Empty;
        [ObservableProperty] private DateTime _scheduledStartDate = DateTime.Now;
        [ObservableProperty] private DateTime _scheduledEndDate = DateTime.Now.AddDays(1);
        [ObservableProperty] private WorkOrderStatus _status = WorkOrderStatus.Open;
        [ObservableProperty] private string _resolutionDescription = string.Empty;
        [ObservableProperty] private string _rootCause = string.Empty;
        [ObservableProperty] private decimal _estimatedCost;
        [ObservableProperty] private decimal _actualCost;
        [ObservableProperty] private string _notes = string.Empty;

        public WorkOrderPriority[] PriorityOptions => (WorkOrderPriority[])Enum.GetValues(typeof(WorkOrderPriority));
        public WorkOrderStatus[] StatusOptions => (WorkOrderStatus[])Enum.GetValues(typeof(WorkOrderStatus));

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(FaultDescription))
            {
                await _dialogService.ShowMessageAsync("Fault description is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingWorkOrder != null)
                {
                    EditingWorkOrder.FaultDescription = FaultDescription;
                    EditingWorkOrder.Priority = Priority;
                    EditingWorkOrder.DeviceCategory = DeviceCategory;
                    EditingWorkOrder.AssignedTo = AssignedTo;
                    EditingWorkOrder.IsExternalContractor = IsExternalContractor;
                    EditingWorkOrder.ContractorName = ContractorName;
                    EditingWorkOrder.ScheduledStartDate = ScheduledStartDate;
                    EditingWorkOrder.ScheduledEndDate = ScheduledEndDate;
                    EditingWorkOrder.Status = Status;
                    EditingWorkOrder.ResolutionDescription = ResolutionDescription;
                    EditingWorkOrder.RootCause = RootCause;
                    EditingWorkOrder.EstimatedCost = EstimatedCost;
                    EditingWorkOrder.ActualCost = ActualCost;
                    EditingWorkOrder.Notes = Notes;
                    EditingWorkOrder.UpdatedAt = DateTime.Now;

                    await _workOrderService.UpdateWorkOrderAsync(EditingWorkOrder);
                    StatusMessage = $"Updated: {EditingWorkOrder.WorkOrderNumber}";
                }
                else
                {
                    var workOrder = new WorkOrder
                    {
                        DeviceId = DeviceId,
                        ReportedBy = ReportedBy,
                        ReportDate = DateTime.Now,
                        FaultDescription = FaultDescription,
                        Priority = Priority,
                        DeviceCategory = DeviceCategory,
                        AssignedTo = AssignedTo,
                        IsExternalContractor = IsExternalContractor,
                        ContractorName = ContractorName,
                        ScheduledStartDate = ScheduledStartDate,
                        ScheduledEndDate = ScheduledEndDate,
                        Status = Status,
                        EstimatedCost = EstimatedCost,
                        ActualCost = ActualCost,
                        Notes = Notes,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _workOrderService.CreateWorkOrderAsync(workOrder);
                    StatusMessage = $"Created: {workOrder.WorkOrderNumber}";
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

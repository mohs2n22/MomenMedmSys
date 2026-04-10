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
    public partial class WorkOrdersViewModel : ViewModelBase
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private MainViewModel? _mainVM;

        public WorkOrdersViewModel(IWorkOrderService workOrderService, IDeviceService deviceService, IDialogService dialogService)
        {
            _workOrderService = workOrderService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            Title = "Work Orders";
            LoadWorkOrdersCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<WorkOrder> WorkOrders { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        [ObservableProperty] private WorkOrder? _selectedWorkOrder;
        [ObservableProperty] private int _openCount;
        [ObservableProperty] private int _overdueCount;

        [RelayCommand]
        private async Task LoadWorkOrders()
        {
            IsLoading = true;
            try
            {
                WorkOrders.Clear();
                var all = await _workOrderService.GetAllWorkOrdersAsync();
                foreach (var w in all) WorkOrders.Add(w);

                Devices.Clear();
                var devices = await _deviceService.GetAllDevicesAsync();
                foreach (var d in devices) Devices.Add(d);

                OpenCount = await _workOrderService.GetOpenWorkOrderCountAsync();
                OverdueCount = await _workOrderService.GetOverdueWorkOrderCountAsync();

                StatusMessage = $"Loaded {WorkOrders.Count} work orders";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task DeleteWorkOrder()
        {
            if (SelectedWorkOrder == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync("Delete this work order?", "Confirm");
            if (confirmed)
            {
                await _workOrderService.DeleteWorkOrderAsync(SelectedWorkOrder.Id);
                WorkOrders.Remove(SelectedWorkOrder);
                SelectedWorkOrder = null;
                StatusMessage = "Work order deleted";
            }
        }
    }
}

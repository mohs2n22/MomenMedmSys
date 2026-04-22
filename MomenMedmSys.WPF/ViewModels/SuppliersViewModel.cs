using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class SuppliersViewModel : ViewModelBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IDialogService _dialogService;
        private readonly Func<SupplierFormViewModel> _formVmFactory;
        private MainViewModel? _mainVM;

        public SuppliersViewModel(ISupplierService supplierService, IDialogService dialogService,
            Func<SupplierFormViewModel> formVmFactory)
        {
            _supplierService = supplierService;
            _dialogService = dialogService;
            _formVmFactory = formVmFactory;
            Title = "Suppliers";
        }

        public async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                var suppliers = await _supplierService.GetAllAsync();
                Suppliers.Clear();
                foreach (var s in suppliers) Suppliers.Add(s);

                TotalCount = Suppliers.Count;
                ApprovedCount = await _supplierService.GetApprovedCountAsync();
                StatusMessage = $"Loaded {TotalCount} suppliers";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        public ObservableCollection<Supplier> Suppliers { get; } = new();
        public int TotalCount { get; private set; }
        public int ApprovedCount { get; private set; }

        [RelayCommand]
        private async Task AddSupplier()
        {
            var formVm = _formVmFactory();
            formVm.SetAddMode();
            _mainVM?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task EditSupplier(Supplier? supplier)
        {
            if (supplier == null) return;
            var formVm = _formVmFactory();
            formVm.SetEditMode(supplier);
            _mainVM?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteSupplier(Supplier? supplier)
        {
            if (supplier == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync($"Delete supplier '{supplier.CompanyName}'?", "Confirm");
            if (confirmed)
            {
                await _supplierService.DeleteAsync(supplier.Id);
                StatusMessage = $"Deleted: {supplier.CompanyName}";
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task Refresh() => await LoadDataAsync();

        public void SetMainViewModel(MainViewModel mainVM) => _mainVM = mainVM;
    }
}

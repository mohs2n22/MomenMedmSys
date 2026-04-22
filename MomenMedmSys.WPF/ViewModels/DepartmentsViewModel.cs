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
    public partial class DepartmentsViewModel : ViewModelBase
    {
        private readonly IDepartmentService _deptService;
        private readonly IDialogService _dialogService;
        private readonly Func<DepartmentFormViewModel> _formVmFactory;
        private MainViewModel? _mainVM;

        public DepartmentsViewModel(IDepartmentService deptService, IDialogService dialogService,
            Func<DepartmentFormViewModel> formVmFactory)
        {
            _deptService = deptService;
            _dialogService = dialogService;
            _formVmFactory = formVmFactory;
            Title = "Departments";
        }

        public async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                var depts = await _deptService.GetAllAsync();
                Departments.Clear();
                foreach (var d in depts) Departments.Add(d);

                TotalCount = Departments.Count;
                ActiveCount = await _deptService.GetActiveCountAsync();
                StatusMessage = $"Loaded {TotalCount} departments";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        public ObservableCollection<Department> Departments { get; } = new();
        public int TotalCount { get; private set; }
        public int ActiveCount { get; private set; }

        [RelayCommand]
        private async Task AddDepartment()
        {
            var formVm = _formVmFactory();
            formVm.SetAddMode();
            _mainVM?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task EditDepartment(Department? dept)
        {
            if (dept == null) return;
            var formVm = _formVmFactory();
            formVm.SetEditMode(dept);
            _mainVM?.NavigateTo(formVm);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteDepartment(Department? dept)
        {
            if (dept == null) return;
            var confirmed = await _dialogService.ShowConfirmAsync($"Delete department '{dept.Name}'?", "Confirm");
            if (confirmed)
            {
                await _deptService.DeleteAsync(dept.Id);
                StatusMessage = $"Deleted: {dept.Name}";
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task Refresh() => await LoadDataAsync();

        public void SetMainViewModel(MainViewModel mainVM) => _mainVM = mainVM;
    }
}

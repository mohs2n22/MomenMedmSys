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
    public partial class DepartmentFormViewModel : ViewModelBase
    {
        private readonly IDepartmentService _deptService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public Department? EditingDepartment { get; private set; }

        public DepartmentFormViewModel(IDepartmentService deptService, IDialogService dialogService)
        {
            _deptService = deptService;
            _dialogService = dialogService;
            Title = "Add Department";
        }

        public void SetEditMode(Department dept)
        {
            Mode = "Edit";
            EditingDepartment = dept;
            Title = "Edit Department";

            DepartmentCode = dept.DepartmentCode;
            Name = dept.Name;
            Description = dept.Description;
            Manager = dept.Manager;
            Building = dept.Building;
            Floor = dept.Floor;
            ContactPhone = dept.ContactPhone;
            ContactEmail = dept.ContactEmail;
            Budget = dept.Budget;
            IsActive = dept.IsActive;
            StatusMessage = $"Editing: {dept.Name}";
        }

        public void SetAddMode()
        {
            Mode = "Add";
            EditingDepartment = null;
            Title = "Add Department";
            DepartmentCode = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            Manager = string.Empty;
            Building = string.Empty;
            Floor = string.Empty;
            ContactPhone = string.Empty;
            ContactEmail = string.Empty;
            Budget = 0;
            IsActive = true;
            StatusMessage = "Fill in department details";
        }

        [ObservableProperty] private string _departmentCode = string.Empty;
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _manager = string.Empty;
        [ObservableProperty] private string _building = string.Empty;
        [ObservableProperty] private string _floor = string.Empty;
        [ObservableProperty] private string _contactPhone = string.Empty;
        [ObservableProperty] private string _contactEmail = string.Empty;
        [ObservableProperty] private decimal _budget;
        [ObservableProperty] private bool _isActive = true;

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await _dialogService.ShowMessageAsync("Department name is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingDepartment != null)
                {
                    EditingDepartment.DepartmentCode = DepartmentCode;
                    EditingDepartment.Name = Name;
                    EditingDepartment.Description = Description;
                    EditingDepartment.Manager = Manager;
                    EditingDepartment.Building = Building;
                    EditingDepartment.Floor = Floor;
                    EditingDepartment.ContactPhone = ContactPhone;
                    EditingDepartment.ContactEmail = ContactEmail;
                    EditingDepartment.Budget = Budget;
                    EditingDepartment.IsActive = IsActive;
                    EditingDepartment.UpdatedAt = DateTime.Now;

                    await _deptService.UpdateAsync(EditingDepartment);
                    StatusMessage = $"Updated: {Name}";
                }
                else
                {
                    var dept = new Department
                    {
                        DepartmentCode = DepartmentCode,
                        Name = Name,
                        Description = Description,
                        Manager = Manager,
                        Building = Building,
                        Floor = Floor,
                        ContactPhone = ContactPhone,
                        ContactEmail = ContactEmail,
                        Budget = Budget,
                        IsActive = IsActive,
                        CreatedAt = DateTime.Now
                    };

                    await _deptService.CreateAsync(dept);
                    StatusMessage = $"Created: {Name}";
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

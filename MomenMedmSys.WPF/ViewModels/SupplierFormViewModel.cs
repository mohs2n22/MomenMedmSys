using System;
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
    public partial class SupplierFormViewModel : ViewModelBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IDialogService _dialogService;

        public string Mode { get; private set; } = "Add";
        public Supplier? EditingSupplier { get; private set; }

        public SupplierFormViewModel(ISupplierService supplierService, IDialogService dialogService)
        {
            _supplierService = supplierService;
            _dialogService = dialogService;
            Title = "Add Supplier";
        }

        public void SetEditMode(Supplier supplier)
        {
            Mode = "Edit";
            EditingSupplier = supplier;
            Title = "Edit Supplier";

            SupplierCode = supplier.SupplierCode;
            CompanyName = supplier.CompanyName;
            ContactPerson = supplier.ContactPerson;
            Email = supplier.Email;
            Phone = supplier.Phone;
            Address = supplier.Address;
            City = supplier.City;
            Country = supplier.Country;
            Website = supplier.Website;
            Rating = supplier.Rating;
            IsApproved = supplier.IsApproved;
            LeadTimeDays = supplier.LeadTimeDays;
            PaymentTerms = supplier.PaymentTerms;
            Notes = supplier.Notes;
            StatusMessage = $"Editing: {supplier.CompanyName}";
        }

        public void SetAddMode()
        {
            Mode = "Add";
            EditingSupplier = null;
            Title = "Add Supplier";
            SupplierCode = string.Empty;
            CompanyName = string.Empty;
            ContactPerson = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            City = string.Empty;
            Country = string.Empty;
            Website = string.Empty;
            Rating = 3;
            IsApproved = false;
            LeadTimeDays = 30;
            PaymentTerms = string.Empty;
            Notes = string.Empty;
            StatusMessage = "Fill in supplier details";
        }

        [ObservableProperty] private string _supplierCode = string.Empty;
        [ObservableProperty] private string _companyName = string.Empty;
        [ObservableProperty] private string _contactPerson = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _phone = string.Empty;
        [ObservableProperty] private string _address = string.Empty;
        [ObservableProperty] private string _city = string.Empty;
        [ObservableProperty] private string _country = string.Empty;
        [ObservableProperty] private string _website = string.Empty;
        [ObservableProperty] private int _rating = 3;
        [ObservableProperty] private bool _isApproved;
        [ObservableProperty] private int _leadTimeDays = 30;
        [ObservableProperty] private string _paymentTerms = string.Empty;
        [ObservableProperty] private string _notes = string.Empty;

        public int[] RatingOptions => new[] { 1, 2, 3, 4, 5 };

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                await _dialogService.ShowMessageAsync("Company name is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingSupplier != null)
                {
                    EditingSupplier.SupplierCode = SupplierCode;
                    EditingSupplier.CompanyName = CompanyName;
                    EditingSupplier.ContactPerson = ContactPerson;
                    EditingSupplier.Email = Email;
                    EditingSupplier.Phone = Phone;
                    EditingSupplier.Address = Address;
                    EditingSupplier.City = City;
                    EditingSupplier.Country = Country;
                    EditingSupplier.Website = Website;
                    EditingSupplier.Rating = Rating;
                    EditingSupplier.IsApproved = IsApproved;
                    EditingSupplier.LeadTimeDays = LeadTimeDays;
                    EditingSupplier.PaymentTerms = PaymentTerms;
                    EditingSupplier.Notes = Notes;
                    EditingSupplier.UpdatedAt = DateTime.Now;

                    await _supplierService.UpdateAsync(EditingSupplier);
                    StatusMessage = $"Updated: {CompanyName}";
                }
                else
                {
                    var supplier = new Supplier
                    {
                        SupplierCode = string.IsNullOrWhiteSpace(SupplierCode) ? $"SUP-{DateTime.Now:yyyyMMdd}" : SupplierCode,
                        CompanyName = CompanyName,
                        ContactPerson = ContactPerson,
                        Email = Email,
                        Phone = Phone,
                        Address = Address,
                        City = City,
                        Country = Country,
                        Website = Website,
                        Rating = Rating,
                        IsApproved = IsApproved,
                        LeadTimeDays = LeadTimeDays,
                        PaymentTerms = PaymentTerms,
                        Notes = Notes,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _supplierService.CreateAsync(supplier);
                    StatusMessage = $"Created: {CompanyName}";
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

using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class SparePartFormViewModel : ViewModelBase
    {
        private readonly ISparePartService _sparePartService;
        private readonly IDialogService _dialogService;
        

        public string Mode { get; private set; } = "Add";
        public SparePart? EditingPart { get; private set; }

        public SparePartFormViewModel(ISparePartService sparePartService, IDialogService dialogService)
        {
            _sparePartService = sparePartService;
            _dialogService = dialogService;
            
            Title = "Add Spare Part";
        }

        public void SetEditMode(SparePart part)
        {
            Mode = "Edit";
            EditingPart = part;
            Title = "Edit Spare Part";
            PartNumber = part.PartNumber;
            PartName = part.PartName;
            Description = part.Description;
            Category = part.Category;
            SupplierName = part.SupplierName;
            Manufacturer = part.Manufacturer;
            CurrentStock = part.CurrentStock;
            MinimumStock = part.MinimumStock;
            MaximumStock = part.MaximumStock;
            ReorderPoint = part.ReorderPoint;
            UnitCost = part.UnitCost;
            StorageLocation = part.StorageLocation;
            IsCritical = part.IsCritical;
            IsObsolete = part.IsObsolete;
            StatusMessage = $"Editing: {part.PartName}";
        }

        public void SetAddMode()
        {
            Mode = "Add";
            EditingPart = null;
            Title = "Add Spare Part";
            PartNumber = "SP-NEW";
            PartName = string.Empty;
            Description = string.Empty;
            Category = string.Empty;
            SupplierName = string.Empty;
            Manufacturer = string.Empty;
            CurrentStock = 0;
            MinimumStock = 0;
            MaximumStock = 0;
            ReorderPoint = 0;
            UnitCost = 0;
            StorageLocation = string.Empty;
            IsCritical = false;
            IsObsolete = false;
            StatusMessage = "Fill in spare part details";
        }

        [ObservableProperty] private string _partNumber = "SP-NEW";
        [ObservableProperty] private string _partName = string.Empty;
        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _category = string.Empty;
        [ObservableProperty] private string _supplierName = string.Empty;
        [ObservableProperty] private string _manufacturer = string.Empty;
        [ObservableProperty] private int _currentStock;
        [ObservableProperty] private int _minimumStock;
        [ObservableProperty] private int _maximumStock;
        [ObservableProperty] private int _reorderPoint;
        [ObservableProperty] private decimal _unitCost;
        [ObservableProperty] private string _storageLocation = string.Empty;
        [ObservableProperty] private bool _isCritical;
        [ObservableProperty] private bool _isObsolete;

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(PartName))
            {
                await _dialogService.ShowMessageAsync("Part name is required.", "Validation Error");
                return;
            }

            try
            {
                if (Mode == "Edit" && EditingPart != null)
                {
                    EditingPart.PartNumber = PartNumber;
                    EditingPart.PartName = PartName;
                    EditingPart.Description = Description;
                    EditingPart.Category = Category;
                    EditingPart.SupplierName = SupplierName;
                    EditingPart.Manufacturer = Manufacturer;
                    EditingPart.CurrentStock = CurrentStock;
                    EditingPart.MinimumStock = MinimumStock;
                    EditingPart.MaximumStock = MaximumStock;
                    EditingPart.ReorderPoint = ReorderPoint;
                    EditingPart.UnitCost = UnitCost;
                    EditingPart.StorageLocation = StorageLocation;
                    EditingPart.IsCritical = IsCritical;
                    EditingPart.IsObsolete = IsObsolete;
                    EditingPart.UpdatedAt = DateTime.Now;

                    await _sparePartService.UpdatePartAsync(EditingPart);
                    StatusMessage = $"Updated: {PartName}";
                }
                else
                {
                    var part = new SparePart
                    {
                        PartNumber = PartNumber,
                        PartName = PartName,
                        Description = Description,
                        Category = Category,
                        SupplierName = SupplierName,
                        Manufacturer = Manufacturer,
                        CurrentStock = CurrentStock,
                        MinimumStock = MinimumStock,
                        MaximumStock = MaximumStock,
                        ReorderPoint = ReorderPoint,
                        UnitCost = UnitCost,
                        StorageLocation = StorageLocation,
                        IsCritical = IsCritical,
                        IsObsolete = IsObsolete,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    await _sparePartService.CreatePartAsync(part);
                    StatusMessage = $"Created: {PartName}";
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

using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    public partial class StaffViewModel : ViewModelBase
    {
        private readonly IStaffService _staffService;
        private readonly ITrainingService _trainingService;
        private readonly IDialogService _dialogService;
        private MainViewModel? _mainVM;

        public StaffViewModel(IStaffService staffService, ITrainingService trainingService, IDialogService dialogService)
        {
            _staffService = staffService;
            _trainingService = trainingService;
            _dialogService = dialogService;
            Title = "Staff & Training";
            LoadStaffCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM) { _mainVM = mainVM; }

        public ObservableCollection<StaffMember> StaffMembers { get; } = new();
        public ObservableCollection<TrainingRecord> TrainingRecords { get; } = new();

        [ObservableProperty] private StaffMember? _selectedStaff;
        [ObservableProperty] private TrainingRecord? _selectedTraining;
        [ObservableProperty] private int _activeStaffCount;
        [ObservableProperty] private int _expiredTrainingCount;
        [ObservableProperty] private int _tabIndex;

        [RelayCommand]
        private async Task LoadStaff()
        {
            IsLoading = true;
            try
            {
                StaffMembers.Clear();
                var all = await _staffService.GetAllStaffAsync();
                foreach (var s in all) StaffMembers.Add(s);
                var active = await _staffService.GetActiveStaffAsync();
                ActiveStaffCount = active.Count();

                StatusMessage = $"Loaded {StaffMembers.Count} staff members";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        [RelayCommand]
        private async Task LoadTraining()
        {
            IsLoading = true;
            try
            {
                TrainingRecords.Clear();
                var all = await _trainingService.GetAllTrainingRecordsAsync();
                foreach (var t in all) TrainingRecords.Add(t);

                var expired = await _trainingService.GetExpiredTrainingAsync();
                ExpiredTrainingCount = expired.Count();
                StatusMessage = $"Loaded {TrainingRecords.Count} training records";
            }
            catch (Exception ex) { StatusMessage = $"Error: {ex.Message}"; }
            finally { IsLoading = false; }
        }
    }
}

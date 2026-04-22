using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class AuditLogViewModel : ViewModelBase
    {
        private readonly IAuditService _auditService;
        private readonly IDialogService _dialogService;
        private MainViewModel? _mainVM;

        public AuditLogViewModel(IAuditService auditService, IDialogService dialogService)
        {
            _auditService = auditService;
            _dialogService = dialogService;
            Title = "Audit Trail";

            // Set default date range to last 30 days
            _startDate = DateTime.Now.AddDays(-30).Date;
            _endDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);

            LoadAuditLogsCommand.Execute(null);
        }

        public void SetMainViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
        }

        public ObservableCollection<AuditLog> AuditLogs { get; } = new();
        public ObservableCollection<AuditLog> FilteredAuditLogs { get; } = new();

        private AuditLog? _selectedAuditLog;
        public AuditLog? SelectedAuditLog
        {
            get => _selectedAuditLog;
            set => SetProperty(ref _selectedAuditLog, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilters();
            }
        }

        private string _filterEntityType = "All";
        public string FilterEntityType
        {
            get => _filterEntityType;
            set
            {
                if (SetProperty(ref _filterEntityType, value))
                    ApplyFilters();
            }
        }

        private string _filterAction = "All";
        public string FilterAction
        {
            get => _filterAction;
            set
            {
                if (SetProperty(ref _filterAction, value))
                    ApplyFilters();
            }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                    ApplyDateFilter();
            }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                    ApplyDateFilter();
            }
        }

        private ObservableCollection<string> _entityTypes = new();
        public ObservableCollection<string> EntityTypes
        {
            get => _entityTypes;
            set => SetProperty(ref _entityTypes, value);
        }

        [RelayCommand]
        private async Task LoadAuditLogs()
        {
            IsLoading = true;
            try
            {
                AuditLogs.Clear();
                var logs = await _auditService.GetAuditLogsAsync(
                    startDate: StartDate,
                    endDate: EndDate);

                foreach (var log in logs)
                    AuditLogs.Add(log);

                // Extract distinct entity types for filter dropdown
                EntityTypes.Clear();
                EntityTypes.Add("All");
                foreach (var type in AuditLogs.Select(a => a.EntityType).Distinct().OrderBy(t => t))
                    EntityTypes.Add(type);

                ApplyFilters();
                StatusMessage = $"Loaded {AuditLogs.Count} audit entries";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading audit logs: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            FilteredAuditLogs.Clear();
            var query = AuditLogs.AsEnumerable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(a =>
                    a.EntityType.ToLower().Contains(search) ||
                    a.UserName.ToLower().Contains(search) ||
                    (a.AffectedRecords != null && a.AffectedRecords.ToLower().Contains(search)) ||
                    a.Action.ToLower().Contains(search));
            }

            // Entity type filter
            if (FilterEntityType != "All")
                query = query.Where(a => a.EntityType == FilterEntityType);

            // Action filter
            if (FilterAction != "All")
                query = query.Where(a => a.Action == FilterAction);

            foreach (var log in query)
                FilteredAuditLogs.Add(log);
        }

        private void ApplyDateFilter()
        {
            // When date range changes, reload from database
            LoadAuditLogsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task ViewDetails()
        {
            if (SelectedAuditLog == null)
            {
                await _dialogService.ShowMessageAsync("Please select an audit log entry to view.", "No Selection");
                return;
            }

            var detailVM = new AuditLogDetailViewModel(SelectedAuditLog);
            _mainVM?.NavigateTo(detailVM);
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                FileName = $"AuditLogs_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            try
            {
                IsLoading = true;
                StatusMessage = "Exporting audit logs...";

                await _auditService.ExportAuditLogsAsync(
                    saveDialog.FileName,
                    entityType: FilterEntityType != "All" ? FilterEntityType : null,
                    startDate: StartDate,
                    endDate: EndDate);

                StatusMessage = $"Exported to {Path.GetFileName(saveDialog.FileName)}";
                await _dialogService.ShowMessageAsync(
                    $"Audit logs exported successfully to:\n{saveDialog.FileName}",
                    "Export Complete");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting: {ex.Message}";
                await _dialogService.ShowMessageAsync(
                    $"Failed to export audit logs:\n{ex.Message}",
                    "Export Error");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SearchText = string.Empty;
            FilterEntityType = "All";
            FilterAction = "All";
            StartDate = DateTime.Now.AddDays(-30).Date;
            EndDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Get display color for action type (for UI badges)
        /// </summary>
        public static string GetActionColor(string action)
        {
            return action.ToLower() switch
            {
                "create" => "#166534",
                "update" => "#1D4ED8",
                "delete" => "#991B1B",
                _ => "#6B7280"
            };
        }

        public static string GetActionBgColor(string action)
        {
            return action.ToLower() switch
            {
                "create" => "#DCFCE7",
                "update" => "#DBEAFE",
                "delete" => "#FEE2E2",
                _ => "#F3F4F6"
            };
        }
    }
}

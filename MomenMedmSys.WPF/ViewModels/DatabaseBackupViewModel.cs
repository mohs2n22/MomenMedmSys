using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Data;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class DatabaseBackupViewModel : ViewModelBase
    {
        private readonly IDatabaseBackupService _backupService;
        private readonly IDialogService _dialogService;
        private readonly AppConfig _appConfig;
        private MainViewModel? _mainVM;

        public void SetMainViewModel(MainViewModel mainVM) => _mainVM = mainVM;

        public DatabaseBackupViewModel(IDatabaseBackupService backupService, IDialogService dialogService, AppConfig config)
        {
            _backupService = backupService;
            _dialogService = dialogService;
            _appConfig = config;
            Title = "Database Backup & Restore";

            // Set default backup directory
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            BackupDirectory = Path.Combine(appDataDir, "MomenMedmSys", "Backups");

            // Default settings
            AutoBackupEnabled = false;
            BackupFrequency = BackupFrequency.Daily;
            RetentionCount = 10;
            CompressBackups = true;

            // Load initial data
            _ = RefreshAll();
        }

        // ─── Backup History ───
        public ObservableCollection<BackupInfo> BackupHistory { get; } = new();

        // ─── Current Database Info ───
        [ObservableProperty] private string _currentDatabasePath = string.Empty;
        [ObservableProperty] private string _currentDatabaseSize = "Unknown";
        [ObservableProperty] private string _currentDatabaseLastModified = "Unknown";

        // ─── Backup Settings ───
        [ObservableProperty] private string _backupDirectory = string.Empty;
        [ObservableProperty] private bool _autoBackupEnabled;
        [ObservableProperty] private BackupFrequency _backupFrequency;
        [ObservableProperty] private int _retentionCount = 10;
        [ObservableProperty] private bool _compressBackups;

        // ─── State ───
        [ObservableProperty] private bool _isBackingUp;
        [ObservableProperty] private bool _isRestoring;
        [ObservableProperty] private double _progressValue;
        [ObservableProperty] private bool _isProgressIndeterminate;
        [ObservableProperty] private BackupInfo? _selectedBackup;
        [ObservableProperty] private string _availableDiskSpace = "Unknown";
        [ObservableProperty] private int _activeTabIndex;

        // ─── Commands ───

        [RelayCommand]
        private async Task CreateBackup()
        {
            if (IsBackingUp) return;

            IsBackingUp = true;
            IsProgressIndeterminate = true;
            StatusMessage = "Creating backup...";

            try
            {
                if (!Directory.Exists(BackupDirectory))
                    Directory.CreateDirectory(BackupDirectory);

                var backup = await _backupService.CreateTimestampedBackupAsync(BackupDirectory);

                if (CompressBackups && !backup.IsCompressed)
                {
                    StatusMessage = "Compressing backup...";
                    backup = await _backupService.CompressBackupAsync(backup.FilePath);
                }

                StatusMessage = $"Backup created: {backup.FileName} ({FormatFileSize(backup.FileSizeBytes)})";
                await RefreshBackupHistory();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Backup failed: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to create backup:\n{ex.Message}", "Backup Error");
            }
            finally
            {
                IsBackingUp = false;
                IsProgressIndeterminate = false;
            }
        }

        [RelayCommand]
        private async Task RestoreBackup()
        {
            if (SelectedBackup == null)
            {
                await _dialogService.ShowMessageAsync("Please select a backup to restore.", "No Backup Selected");
                return;
            }

            if (!SelectedBackup.IsValid)
            {
                var validate = await _backupService.ValidateBackupAsync(SelectedBackup.FilePath);
                if (!validate.IsValid)
                {
                    await _dialogService.ShowMessageAsync(
                        $"This backup is not valid:\n{validate.ValidationMessage}", "Invalid Backup");
                    return;
                }
                SelectedBackup = validate;
            }

            // Confirm restore
            var confirmed = await _dialogService.ShowConfirmAsync(
                $"You are about to restore from:\n\n{SelectedBackup.FileName}\nCreated: {SelectedBackup.CreatedAt:yyyy-MM-dd HH:mm:ss}\nSize: {FormatFileSize(SelectedBackup.FileSizeBytes)}\n\nThis will REPLACE the current database. All unsaved changes will be lost.\n\nThe application will need to restart after restore.\n\nContinue?",
                "Confirm Restore");

            if (!confirmed) return;

            IsRestoring = true;
            IsProgressIndeterminate = true;
            StatusMessage = "Restoring backup...";

            try
            {
                var dbPath = GetDatabaseFilePath();
                await _backupService.RestoreBackupAsync(SelectedBackup.FilePath, dbPath);

                StatusMessage = $"Restored from {SelectedBackup.FileName}. Please restart the application.";
                await _dialogService.ShowMessageAsync(
                    $"Backup restored successfully from:\n{SelectedBackup.FileName}\n\nPlease restart the application for changes to take effect.",
                    "Restore Complete");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Restore failed: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to restore backup:\n{ex.Message}", "Restore Error");
            }
            finally
            {
                IsRestoring = false;
                IsProgressIndeterminate = false;
            }
        }

        [RelayCommand]
        private async Task DeleteBackup()
        {
            if (SelectedBackup == null)
            {
                await _dialogService.ShowMessageAsync("Please select a backup to delete.", "No Backup Selected");
                return;
            }

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Delete backup:\n{SelectedBackup.FileName}?\n\nThis action cannot be undone.",
                "Confirm Delete");

            if (!confirmed) return;

            try
            {
                File.Delete(SelectedBackup.FilePath);
                StatusMessage = $"Deleted: {SelectedBackup.FileName}";
                await RefreshBackupHistory();
                SelectedBackup = null;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Delete failed: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to delete backup:\n{ex.Message}", "Delete Error");
            }
        }

        [RelayCommand]
        private async Task CleanupOldBackups()
        {
            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Delete all backups except the most recent {RetentionCount}?",
                "Confirm Cleanup");

            if (!confirmed) return;

            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    await _dialogService.ShowMessageAsync("Backup directory does not exist.", "Nothing to Clean Up");
                    return;
                }

                var deleted = await _backupService.CleanupOldBackupsAsync(BackupDirectory, RetentionCount);
                StatusMessage = $"Cleanup complete. Deleted {deleted} old backup(s).";
                await RefreshBackupHistory();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Cleanup failed: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Failed to cleanup backups:\n{ex.Message}", "Cleanup Error");
            }
        }

        [RelayCommand]
        private async Task ValidateSelectedBackup()
        {
            if (SelectedBackup == null)
            {
                await _dialogService.ShowMessageAsync("Please select a backup to validate.", "No Backup Selected");
                return;
            }

            StatusMessage = $"Validating {SelectedBackup.FileName}...";
            try
            {
                var result = await _backupService.ValidateBackupAsync(SelectedBackup.FilePath);
                SelectedBackup = result;

                if (result.IsValid)
                    StatusMessage = $"Valid: {result.ValidationMessage}";
                else
                    StatusMessage = $"Invalid: {result.ValidationMessage}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Validation error: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task TestBackup()
        {
            StatusMessage = "Running test backup...";
            IsBackingUp = true;

            try
            {
                var testDir = Path.Combine(Path.GetTempPath(), "MomenMedmSys_TestBackup");
                if (!Directory.Exists(testDir))
                    Directory.CreateDirectory(testDir);

                var testBackup = await _backupService.CreateTimestampedBackupAsync(testDir);

                // Validate it
                var validated = await _backupService.ValidateBackupAsync(testBackup.FilePath);

                // Clean up test file
                if (File.Exists(testBackup.FilePath))
                    File.Delete(testBackup.FilePath);

                if (validated.IsValid)
                {
                    StatusMessage = "Test backup successful - backup and validation working correctly.";
                    await _dialogService.ShowMessageAsync(
                        $"Test backup created and validated successfully.\n\nBackup size: {FormatFileSize(testBackup.FileSizeBytes)}\nValidation: {validated.ValidationMessage}",
                        "Test Successful");
                }
                else
                {
                    StatusMessage = $"Test backup failed validation: {validated.ValidationMessage}";
                    await _dialogService.ShowMessageAsync($"Test backup failed:\n{validated.ValidationMessage}", "Test Failed");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Test failed: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Test backup failed:\n{ex.Message}", "Test Error");
            }
            finally
            {
                IsBackingUp = false;
            }
        }

        [RelayCommand]
        private async Task BrowseBackupDirectory()
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog();
            if (!string.IsNullOrEmpty(BackupDirectory) && Directory.Exists(BackupDirectory))
                dialog.InitialDirectory = BackupDirectory;

            if (dialog.ShowDialog() == true)
            {
                BackupDirectory = dialog.FolderName;
                await RefreshBackupHistory();
            }
        }

        [RelayCommand]
        private async Task RefreshAll()
        {
            await RefreshDatabaseInfo();
            await RefreshBackupHistory();
        }

        private async Task RefreshDatabaseInfo()
        {
            try
            {
                var dbInfo = await _backupService.GetDatabaseInfoAsync();
                CurrentDatabasePath = dbInfo.FilePath;
                CurrentDatabaseSize = FormatFileSize(dbInfo.FileSizeBytes);
                CurrentDatabaseLastModified = FormatRelativeTime(dbInfo.LastModified);

                var availableSpace = await _backupService.GetAvailableDiskSpaceAsync(dbInfo.FilePath);
                AvailableDiskSpace = FormatFileSize(availableSpace);
            }
            catch (Exception ex)
            {
                CurrentDatabasePath = "Error loading database info";
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private async Task RefreshBackupHistory()
        {
            try
            {
                if (!Directory.Exists(BackupDirectory))
                {
                    BackupHistory.Clear();
                    return;
                }

                var backups = await _backupService.GetBackupHistoryAsync(BackupDirectory);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    BackupHistory.Clear();
                    foreach (var backup in backups)
                    {
                        BackupHistory.Add(backup);
                    }
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading backup history: {ex.Message}";
            }
        }

        // ─── Helpers ───

        private string GetDatabaseFilePath()
        {
            var connectionString = _appConfig.Database.ConnectionString;
            var parts = connectionString.Split(new[] { "Data Source=" }, StringSplitOptions.None);
            if (parts.Length < 2)
                throw new InvalidOperationException("Could not parse database connection string.");

            var dbPath = parts[1].Trim();
            var semicolonIndex = dbPath.IndexOf(';');
            if (semicolonIndex > 0)
                dbPath = dbPath.Substring(0, semicolonIndex);

            return dbPath;
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private static string FormatRelativeTime(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;

            if (span.TotalSeconds < 60)
                return "just now";
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes} minute{(span.TotalMinutes >= 2 ? "s" : "")} ago";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours} hour{(span.TotalHours >= 2 ? "s" : "")} ago";
            if (span.TotalDays < 7)
                return $"{(int)span.TotalDays} day{(span.TotalDays >= 2 ? "s" : "")} ago";
            if (span.TotalDays < 30)
                return $"{(int)(span.TotalDays / 7)} week{(span.TotalDays / 7 >= 2 ? "s" : "")} ago";
            if (span.TotalDays < 365)
                return $"{(int)(span.TotalDays / 30)} month{(span.TotalDays / 30 >= 2 ? "s" : "")} ago";

            return $"{(int)(span.TotalDays / 365)} year{(span.TotalDays / 365 >= 2 ? "s" : "")} ago";
        }
    }
}

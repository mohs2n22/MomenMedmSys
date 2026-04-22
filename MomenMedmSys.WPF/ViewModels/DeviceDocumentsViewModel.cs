using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Services;
using MomenMedmSys.WPF.Services;
using MomenMedmSys.WPF.ViewModels.Base;
using Microsoft.Win32;

namespace MomenMedmSys.WPF.ViewModels
{
    public partial class DeviceDocumentsViewModel : ViewModelBase
    {
        private readonly IDocumentService _documentService;
        private readonly IDeviceService _deviceService;
        private readonly IDialogService _dialogService;
        private readonly CurrentUserService _currentUserService;
        private MainViewModel? _mainVM;

        public DeviceDocumentsViewModel(IDocumentService documentService, IDeviceService deviceService,
            IDialogService dialogService, CurrentUserService currentUserService)
        {
            _documentService = documentService;
            _deviceService = deviceService;
            _dialogService = dialogService;
            _currentUserService = currentUserService;
            Title = "Device Documents";
            LoadDocumentsCommand.Execute(null);
        }

        /// <summary>
        /// Set MainViewModel reference after construction to avoid circular DI
        /// </summary>
        public void SetMainViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
        }

        public ObservableCollection<DeviceDocument> Documents { get; } = new();
        public ObservableCollection<DeviceDocument> FilteredDocuments { get; } = new();
        public ObservableCollection<MedicalDevice> Devices { get; } = new();

        private DeviceDocument? _selectedDocument;
        public DeviceDocument? SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                if (SetProperty(ref _selectedDocument, value))
                {
                    OnPropertyChanged(nameof(IsDocumentSelected));
                    OnPropertyChanged(nameof(DocumentDetailsVisible));
                }
            }
        }

        public bool IsDocumentSelected => SelectedDocument != null;
        public Visibility DocumentDetailsVisible => IsDocumentSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NoDocumentSelectedVisibility => IsDocumentSelected ? Visibility.Collapsed : Visibility.Visible;

        private int? _selectedDeviceId;
        public int? SelectedDeviceId
        {
            get => _selectedDeviceId;
            set
            {
                if (SetProperty(ref _selectedDeviceId, value))
                {
                    ApplyFilter();
                }
            }
        }

        private MedicalDevice? _selectedDevice;
        public MedicalDevice? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (SetProperty(ref _selectedDevice, value))
                {
                    SelectedDeviceId = value?.Id;
                }
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        private string _filterDocumentType = "All";
        public string FilterDocumentType
        {
            get => _filterDocumentType;
            set
            {
                if (SetProperty(ref _filterDocumentType, value))
                {
                    ApplyFilter();
                }
            }
        }

        private int _documentCount;
        public int DocumentCount
        {
            get => _documentCount;
            set => SetProperty(ref _documentCount, value);
        }

        private string _totalSizeDisplay = "0 B";
        public string TotalSizeDisplay
        {
            get => _totalSizeDisplay;
            set => SetProperty(ref _totalSizeDisplay, value);
        }

        private DocumentStats? _currentStats;
        public DocumentStats? CurrentStats
        {
            get => _currentStats;
            set => SetProperty(ref _currentStats, value);
        }

        // Document detail panel properties
        public string DocumentTypeName => SelectedDocument?.DocumentType switch
        {
            DocumentType.OperationManual => "Operation Manual",
            DocumentType.MaintenanceManual => "Maintenance Manual",
            DocumentType.WarrantyCertificate => "Warranty Certificate",
            DocumentType.TrainingMaterial => "Training Material",
            DocumentType.RegulatoryCertificate => "Regulatory Certificate",
            DocumentType.CalibrationCertificate => "Calibration Certificate",
            DocumentType.SafetyTestReport => "Safety Test Report",
            DocumentType.TechnicalDrawing => "Technical Drawing",
            DocumentType.SoftwareManual => "Software Manual",
            DocumentType.Other => "Other",
            _ => "Unknown"
        };

        public string DocumentTypeIcon => SelectedDocument?.DocumentType switch
        {
            DocumentType.OperationManual => "\U0001F4D6",
            DocumentType.MaintenanceManual => "\U0001F4D6",
            DocumentType.WarrantyCertificate => "\U0001F6E1",
            DocumentType.TrainingMaterial => "\U0001F4D6",
            DocumentType.RegulatoryCertificate => "\U0001F4DC",
            DocumentType.CalibrationCertificate => "\U0001F4DC",
            DocumentType.SafetyTestReport => "\U0001F4CB",
            DocumentType.TechnicalDrawing => "\U0001F4CA",
            DocumentType.SoftwareManual => "\U0001F4D6",
            DocumentType.Other => "\U0001F4C4",
            _ => "\U0001F4C4"
        };

        public bool IsImageFile => SelectedDocument != null &&
            (SelectedDocument.MimeType.StartsWith("image/") ||
             SelectedDocument.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
             SelectedDocument.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
             SelectedDocument.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
             SelectedDocument.FileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
             SelectedDocument.FileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase));

        public bool IsPdfFile => SelectedDocument != null &&
            (SelectedDocument.MimeType == "application/pdf" ||
             SelectedDocument.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

        public bool CanPreview => IsImageFile || IsPdfFile;

        private const long MaxFileSize = 50 * 1024 * 1024; // 50MB

        [RelayCommand]
        private async Task LoadDocuments()
        {
            IsLoading = true;
            try
            {
                Documents.Clear();
                Devices.Clear();

                // Load all devices for dropdown
                var allDevices = await _deviceService.GetAllDevicesAsync();
                foreach (var device in allDevices)
                    Devices.Add(device);

                // Load all documents
                var allDocs = await _documentService.GetAllDocumentsAsync();
                foreach (var doc in allDocs)
                    Documents.Add(doc);

                ApplyFilter();
                UpdateStats();
                StatusMessage = $"Loaded {Documents.Count} documents";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading documents: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            FilteredDocuments.Clear();
            var query = Documents.AsEnumerable();

            // Filter by device
            if (SelectedDeviceId.HasValue)
            {
                query = query.Where(d => d.DeviceId == SelectedDeviceId.Value);
            }

            // Filter by document type
            if (FilterDocumentType != "All")
            {
                if (Enum.TryParse<DocumentType>(FilterDocumentType, out var docType))
                {
                    query = query.Where(d => d.DocumentType == docType);
                }
            }

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(d =>
                    d.FileName.ToLower().Contains(search) ||
                    d.Description.ToLower().Contains(search) ||
                    d.Version.ToLower().Contains(search) ||
                    d.UploadedBy.ToLower().Contains(search));
            }

            foreach (var doc in query)
                FilteredDocuments.Add(doc);

            UpdateStats();
        }

        private void UpdateStats()
        {
            DocumentCount = FilteredDocuments.Count;
            var totalSize = FilteredDocuments.Sum(d => d.FileSize);
            TotalSizeDisplay = FormatFileSize(totalSize);
            OnPropertyChanged(nameof(StatusBarText));
        }

        public string StatusBarText => $"Showing {DocumentCount} documents | Total size: {TotalSizeDisplay}";

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }

        [RelayCommand]
        private void AddDocument()
        {
            var dialog = new Views.DocumentUploadDialog
            {
                Owner = Application.Current.MainWindow
            };
            var dialogVm = new DocumentUploadDialogViewModel
            {
                DeviceId = SelectedDeviceId,
                Devices = new ObservableCollection<MedicalDevice>(Devices),
                Mode = "Add"
            };
            dialog.DataContext = dialogVm;

            if (dialog.ShowDialog() == true)
            {
                // Reload documents
                LoadDocumentsCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task EditDocument()
        {
            if (SelectedDocument == null)
            {
                await _dialogService.ShowMessageAsync("Please select a document to edit.", "No Selection");
                return;
            }

            var dialog = new Views.DocumentUploadDialog
            {
                Owner = Application.Current.MainWindow
            };
            var dialogVm = new DocumentUploadDialogViewModel
            {
                Mode = "Edit",
                DocumentId = SelectedDocument.Id,
                DocumentName = SelectedDocument.FileName,
                DocumentType = SelectedDocument.DocumentType,
                Version = SelectedDocument.Version,
                Description = SelectedDocument.Description,
                DeviceId = SelectedDocument.DeviceId,
                Devices = new ObservableCollection<MedicalDevice>(Devices),
                ExistingFilePath = SelectedDocument.FilePath
            };
            dialog.DataContext = dialogVm;

            if (dialog.ShowDialog() == true)
            {
                LoadDocumentsCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task DeleteDocument()
        {
            if (SelectedDocument == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync(
                $"Are you sure you want to delete '{SelectedDocument.FileName}'?", "Confirm Delete");

            if (confirmed)
            {
                try
                {
                    // Try to delete physical file if it exists
                    if (File.Exists(SelectedDocument.FilePath))
                    {
                        try { File.Delete(SelectedDocument.FilePath); }
                        catch { /* ignore file delete errors */ }
                    }

                    await _documentService.DeleteDocumentAsync(SelectedDocument.Id);
                    Documents.Remove(SelectedDocument);
                    ApplyFilter();
                    SelectedDocument = null;
                    StatusMessage = "Document deleted";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error deleting document: {ex.Message}";
                }
            }
        }

        [RelayCommand]
        private async Task ViewDocument()
        {
            if (SelectedDocument == null)
            {
                await _dialogService.ShowMessageAsync("Please select a document to view.", "No Selection");
                return;
            }

            if (File.Exists(SelectedDocument.FilePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = SelectedDocument.FilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowMessageAsync($"Could not open file: {ex.Message}", "Error");
                }
            }
            else
            {
                await _dialogService.ShowMessageAsync($"File not found: {SelectedDocument.FilePath}", "File Not Found");
            }
        }

        [RelayCommand]
        private async Task ExportToExcel()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"DeviceDocuments_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    DefaultExt = "xlsx"
                };

                if (saveDialog.ShowDialog() != true) return;

                var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Device Documents");

                // Headers
                var headers = new[] { "Document Name", "Type", "Device ID", "File Size", "Version", "Upload Date", "Uploaded By", "Description", "File Path", "Expiry Date", "Current Version" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                    worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#E3F2FD");
                    worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                }

                // Data
                int row = 2;
                foreach (var doc in FilteredDocuments)
                {
                    worksheet.Cell(row, 1).Value = doc.FileName;
                    worksheet.Cell(row, 2).Value = doc.DocumentType.ToString();
                    worksheet.Cell(row, 3).Value = doc.DeviceId;
                    worksheet.Cell(row, 4).Value = doc.FileSizeDisplay;
                    worksheet.Cell(row, 5).Value = doc.Version;
                    worksheet.Cell(row, 6).Value = doc.UploadDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cell(row, 7).Value = doc.UploadedBy;
                    worksheet.Cell(row, 8).Value = doc.Description;
                    worksheet.Cell(row, 9).Value = doc.FilePath;
                    worksheet.Cell(row, 10).Value = doc.ExpiryDate?.ToString("yyyy-MM-dd") ?? "N/A";
                    worksheet.Cell(row, 11).Value = doc.IsCurrentVersion ? "Yes" : "No";
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save
                workbook.SaveAs(saveDialog.FileName);
                StatusMessage = $"Exported {FilteredDocuments.Count} documents to Excel";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting to Excel: {ex.Message}";
                await _dialogService.ShowMessageAsync($"Error exporting to Excel: {ex.Message}", "Export Error");
            }
        }
    }

    /// <summary>
    /// ViewModel for the Document Upload Dialog
    /// </summary>
    public partial class DocumentUploadDialogViewModel : ObservableObject, System.ComponentModel.INotifyPropertyChanged
    {
        private const long MaxFileSize = 50 * 1024 * 1024; // 50MB

        [ObservableProperty]
        private string _mode = "Add";

        [ObservableProperty]
        private int? _documentId;

        [ObservableProperty]
        private int? _deviceId;

        [ObservableProperty]
        private ObservableCollection<MedicalDevice> _devices = new();

        [ObservableProperty]
        private string _documentName = string.Empty;

        [ObservableProperty]
        private DocumentType _documentType;

        [ObservableProperty]
        private string _version = "1.0";

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string _selectedFilePath = string.Empty;

        [ObservableProperty]
        private string _selectedFileName = string.Empty;

        [ObservableProperty]
        private long _selectedFileSize;

        [ObservableProperty]
        private string _existingFilePath = string.Empty;

        [ObservableProperty]
        private DateTime? _expiryDate;

        [ObservableProperty]
        private bool _isCurrentVersion = true;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private bool _isDragOver;

        public bool HasSelectedFile => SelectedFileSize > 0;

        public string FileSizeDisplay
        {
            get
            {
                if (SelectedFileSize == 0) return "No file selected";
                if (SelectedFileSize < 1024) return $"{SelectedFileSize} B";
                if (SelectedFileSize < 1024 * 1024) return $"{SelectedFileSize / 1024.0:F1} KB";
                return $"{SelectedFileSize / (1024.0 * 1024.0):F1} MB";
            }
        }

        public bool IsFileSizeValid => SelectedFileSize > 0 && SelectedFileSize <= MaxFileSize;

        partial void OnSelectedFileSizeChanged(long value)
        {
            OnPropertyChanged(nameof(FileSizeDisplay));
            OnPropertyChanged(nameof(HasSelectedFile));
            OnPropertyChanged(nameof(IsFileSizeValid));
        }

        [RelayCommand]
        private void BrowseFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*|PDF Files (*.pdf)|*.pdf|Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|Word Documents (*.docx;*.doc)|*.docx;*.doc|Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls",
                Title = "Select Document File"
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedFilePath = dialog.FileName;
                SelectedFileName = Path.GetFileName(dialog.FileName);
                SelectedFileSize = new FileInfo(dialog.FileName).Length;
                OnPropertyChanged(nameof(FileSizeDisplay));
                OnPropertyChanged(nameof(IsFileSizeValid));

                // Auto-set document name if empty
                if (string.IsNullOrWhiteSpace(DocumentName))
                {
                    DocumentName = Path.GetFileNameWithoutExtension(dialog.FileName);
                }

                // Clear any previous errors
                HasError = false;
                ErrorMessage = string.Empty;

                // Validate file size
                if (SelectedFileSize > MaxFileSize)
                {
                    ErrorMessage = $"File size ({FileSizeDisplay}) exceeds maximum allowed size (50 MB)";
                    HasError = true;
                }
            }
        }

        [RelayCommand]
        private void Save()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(DocumentName))
            {
                ErrorMessage = "Document name is required";
                HasError = true;
                return;
            }

            if (!DeviceId.HasValue)
            {
                ErrorMessage = "Please select a device";
                HasError = true;
                return;
            }

            if (Mode == "Add" && string.IsNullOrWhiteSpace(SelectedFilePath))
            {
                ErrorMessage = "Please select a file to upload";
                HasError = true;
                return;
            }

            if (SelectedFileSize > MaxFileSize)
            {
                ErrorMessage = $"File size exceeds maximum allowed size (50 MB)";
                HasError = true;
                return;
            }

            HasError = false;
            ErrorMessage = string.Empty;

            // Signal success (handled by dialog code-behind)
            DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
        }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                OnPropertyChanged(nameof(DialogResult));
            }
        }
    }
}

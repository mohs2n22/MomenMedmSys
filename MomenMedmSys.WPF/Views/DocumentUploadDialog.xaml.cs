using System.IO;
using System.Windows;
using System.Windows.Input;
using MomenMedmSys.WPF.ViewModels;

namespace MomenMedmSys.WPF.Views
{
    public partial class DocumentUploadDialog : Window
    {
        public DocumentUploadDialog()
        {
            InitializeComponent();
        }

        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;

            if (DataContext is DocumentUploadDialogViewModel vm)
            {
                vm.IsDragOver = true;
            }
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            if (DataContext is DocumentUploadDialogViewModel vm)
            {
                vm.IsDragOver = false;
            }
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is DocumentUploadDialogViewModel vm)
            {
                vm.IsDragOver = false;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    var filePath = files[0];

                    // Only accept single files (not directories)
                    if (File.Exists(filePath))
                    {
                        var fileInfo = new FileInfo(filePath);
                        if (DataContext is DocumentUploadDialogViewModel vm2)
                        {
                            vm2.SelectedFilePath = filePath;
                            vm2.SelectedFileName = fileInfo.Name;
                            vm2.SelectedFileSize = fileInfo.Length;

                            // Auto-set document name if empty
                            if (string.IsNullOrWhiteSpace(vm2.DocumentName))
                            {
                                vm2.DocumentName = Path.GetFileNameWithoutExtension(filePath);
                            }

                            // Validate file size
                            const long maxFileSize = 50 * 1024 * 1024; // 50MB
                            if (fileInfo.Length > maxFileSize)
                            {
                                vm2.ErrorMessage = $"File size ({vm2.FileSizeDisplay}) exceeds maximum allowed size (50 MB)";
                                vm2.HasError = true;
                            }
                            else
                            {
                                vm2.HasError = false;
                                vm2.ErrorMessage = string.Empty;
                            }
                        }
                    }
                }
            }
        }

        private void DropZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is DocumentUploadDialogViewModel vm)
            {
                vm.BrowseFileCommand.Execute(null);
            }
        }
    }
}

using System;
using System.Windows;

namespace MomenMedmSys.WPF.Views
{
    /// <summary>
    /// Confirmation dialog shown before restoring a database backup.
    /// </summary>
    public partial class ConfirmRestoreDialog : Window
    {
        public bool Confirmed { get; private set; }

        public ConfirmRestoreDialog()
        {
            InitializeComponent();
            Confirmed = false;
        }

        /// <summary>
        /// Sets the backup details to display in the confirmation dialog.
        /// </summary>
        public void SetBackupInfo(string fileName, DateTime createdAt, long fileSizeBytes)
        {
            BackupFileName.Text = fileName;
            BackupDate.Text = createdAt.ToString("yyyy-MM-dd HH:mm:ss");
            BackupSize.Text = FormatFileSize(fileSizeBytes);
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}

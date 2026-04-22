using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Represents metadata about a backup file.
    /// </summary>
    public class BackupInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCompressed { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the current database file information.
    /// </summary>
    public class DatabaseFileInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime LastModified { get; set; }
    }

    /// <summary>
    /// Backup frequency options.
    /// </summary>
    public enum BackupFrequency
    {
        Daily,
        Weekly,
        Monthly
    }

    /// <summary>
    /// Service for database backup and restore operations — full backup, compressed backup,
    /// scheduled backup, restore, cleanup, validation, and disk space management.
    /// </summary>
    public interface IDatabaseBackupService
    {
        // ─── Backup Methods ───

        /// <summary>
        /// Creates a backup of the database at the specified path.
        /// </summary>
        Task<BackupInfo> CreateBackupAsync(string backupPath);

        /// <summary>
        /// Creates a timestamped backup in the specified directory.
        /// Filename format: MomenMedmSys_YYYYMMDD_HHMMSS.db
        /// </summary>
        Task<BackupInfo> CreateTimestampedBackupAsync(string backupDirectory);

        /// <summary>
        /// Lists all backup files in the specified directory.
        /// </summary>
        Task<IReadOnlyList<BackupInfo>> GetBackupHistoryAsync(string backupDirectory);

        /// <summary>
        /// Compresses a backup file using GZip.
        /// </summary>
        Task<BackupInfo> CompressBackupAsync(string backupPath);

        /// <summary>
        /// Returns the current database file size.
        /// </summary>
        Task<DatabaseFileInfo> GetDatabaseInfoAsync();

        /// <summary>
        /// Returns available disk space for the given path.
        /// </summary>
        Task<long> GetAvailableDiskSpaceAsync(string path);

        // ─── Restore Methods ───

        /// <summary>
        /// Restores the database from a backup file.
        /// </summary>
        Task RestoreBackupAsync(string backupPath, string targetPath);

        /// <summary>
        /// Validates a backup file by attempting to open it and run a simple query.
        /// </summary>
        Task<BackupInfo> ValidateBackupAsync(string backupPath);

        /// <summary>
        /// Returns metadata about a backup file.
        /// </summary>
        Task<BackupInfo> GetBackupInfoAsync(string backupPath);

        // ─── Utility Methods ───

        /// <summary>
        /// Deletes old backups, keeping only the most recent N backups.
        /// </summary>
        Task<int> CleanupOldBackupsAsync(string directory, int keepCount);
    }
}

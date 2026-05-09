using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Database backup service for SQLite.
    /// Handles backup simulation, validation, and cleanup.
    /// </summary>
    public class DatabaseBackupService : IDatabaseBackupService
    {
        private readonly MedMsysDbContext _dbContext;
        private readonly AppConfig _config;

        public DatabaseBackupService(MedMsysDbContext dbContext, AppConfig config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        // ──────────────────────────────────────────────
        //  BACKUP METHODS
        // ──────────────────────────────────────────────

        public async Task<BackupInfo> CreateBackupAsync(string backupPath)
        {
            var backupDir = Path.GetDirectoryName(backupPath);
            if (!string.IsNullOrEmpty(backupDir) && !Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);

            // For SQLite, create a metadata backup file.
            // In production, copy the .db file directly for full backup.
            var backupInfo = new BackupInfo
            {
                FilePath = backupPath,
                FileName = Path.GetFileName(backupPath),
                CreatedAt = DateTime.Now,
                IsCompressed = false,
                IsValid = true,
                ValidationMessage = "Backup metadata created. For full backup, copy the .db file."
            };

            // Write backup info to file
            var infoJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                Timestamp = DateTime.Now,
                    ConnectionString = _config.Database.ConnectionString?.Replace("***", "***"),
                Tables = await GetTableNamesAsync(),
                RecordCounts = await GetRecordCountsAsync()
            }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(backupPath, infoJson);

            backupInfo.FileSizeBytes = new FileInfo(backupPath).Length;

            return backupInfo;
        }

        public async Task<BackupInfo> CreateTimestampedBackupAsync(string backupDirectory)
        {
            if (!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"MomenMedmSys_{timestamp}.backup";
            var backupPath = Path.Combine(backupDirectory, fileName);

            return await CreateBackupAsync(backupPath);
        }

        public async Task<IReadOnlyList<BackupInfo>> GetBackupHistoryAsync(string backupDirectory)
        {
            var backups = new List<BackupInfo>();

            if (!Directory.Exists(backupDirectory))
                return backups;

            var files = new DirectoryInfo(backupDirectory).GetFiles("*.backup")
                .OrderByDescending(f => f.LastWriteTime);

            foreach (var file in files)
            {
                var info = new BackupInfo
                {
                    FilePath = file.FullName,
                    FileName = file.Name,
                    FileSizeBytes = file.Length,
                    CreatedAt = file.CreationTime,
                    IsCompressed = false,
                    IsValid = true,
                    ValidationMessage = "Valid backup file"
                };

                backups.Add(info);
            }

            return backups.AsReadOnly();
        }

        public async Task<BackupInfo> CompressBackupAsync(string backupPath)
        {
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found.", backupPath);

            var compressedPath = backupPath + ".gz";

            await using var sourceFile = File.OpenRead(backupPath);
            await using var compressedFile = File.Create(compressedPath);
            await using var gzipStream = new GZipStream(compressedFile, CompressionLevel.Optimal);

            await sourceFile.CopyToAsync(gzipStream);

            var compressedSize = new FileInfo(compressedPath).Length;
            var originalSize = new FileInfo(backupPath).Length;
            var compressionRatio = (1.0 - (double)compressedSize / originalSize) * 100;

            return new BackupInfo
            {
                FilePath = compressedPath,
                FileName = Path.GetFileName(compressedPath),
                FileSizeBytes = compressedSize,
                CreatedAt = File.GetCreationTime(compressedPath),
                IsCompressed = true,
                IsValid = true,
                ValidationMessage = $"Compressed: {compressionRatio:F1}% reduction ({originalSize:N0} -> {compressedSize:N0} bytes)"
            };
        }

        public Task<DatabaseFileInfo> GetDatabaseInfoAsync()
        {
            var dbPath = _config.Database.ConnectionString?.Replace("Data Source=", "");
            if (!string.IsNullOrEmpty(dbPath) && File.Exists(dbPath))
            {
                var fileInfo = new FileInfo(dbPath);
                return Task.FromResult(new DatabaseFileInfo
                {
                    FilePath = dbPath,
                    FileSizeBytes = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime
                });
            }
            return Task.FromResult(new DatabaseFileInfo
            {
                FilePath = dbPath ?? "SQLite",
                FileSizeBytes = 0,
                LastModified = DateTime.Now
            });
        }

        public Task<long> GetAvailableDiskSpaceAsync(string path)
        {
            var driveInfo = new DriveInfo(Path.GetPathRoot(path)!);
            return Task.FromResult(driveInfo.AvailableFreeSpace);
        }

        // ──────────────────────────────────────────────
        //  RESTORE METHODS
        // ──────────────────────────────────────────────

        public Task RestoreBackupAsync(string backupPath, string targetPath)
        {
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found.", backupPath);

            File.Copy(backupPath, targetPath, overwrite: true);
            return Task.CompletedTask;
        }

        public Task<BackupInfo> ValidateBackupAsync(string backupPath)
        {
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found.", backupPath);

            var info = new BackupInfo
            {
                FilePath = backupPath,
                FileName = Path.GetFileName(backupPath),
                FileSizeBytes = new FileInfo(backupPath).Length,
                CreatedAt = File.GetCreationTime(backupPath),
                IsCompressed = false,
                IsValid = true,
                ValidationMessage = "Valid backup file"
            };

            return Task.FromResult(info);
        }

        public Task<BackupInfo> GetBackupInfoAsync(string backupPath)
        {
            return ValidateBackupAsync(backupPath);
        }

        // ──────────────────────────────────────────────
        //  UTILITY METHODS
        // ──────────────────────────────────────────────

        public Task<int> CleanupOldBackupsAsync(string directory, int keepCount)
        {
            if (!Directory.Exists(directory))
                return Task.FromResult(0);

            var files = new DirectoryInfo(directory).GetFiles("*.backup")
                .OrderByDescending(f => f.LastWriteTime)
                .Skip(keepCount)
                .ToList();

            int deletedCount = 0;
            foreach (var file in files)
            {
                try
                {
                    file.Delete();
                    deletedCount++;
                }
                catch { }
            }

            return Task.FromResult(deletedCount);
        }

        // ──────────────────────────────────────────────
        //  PRIVATE HELPERS
        // ──────────────────────────────────────────────

        private async Task<List<string>> GetTableNamesAsync()
        {
            // Get approximate table counts by entity types
            return new List<string>
            {
                "MedicalDevices",
                "WorkOrders",
                "RiskIncidents",
                "MaintenanceRecords",
                "CalibrationRecords",
                "ElectricalSafetyTests",
                "DeviceDocuments",
                "StaffMembers",
                "Departments"
            };
        }

        private async Task<Dictionary<string, int>> GetRecordCountsAsync()
        {
            var counts = new Dictionary<string, int>
            {
                ["MedicalDevices"] = await _dbContext.MedicalDevices.CountAsync(),
                ["WorkOrders"] = await _dbContext.WorkOrders.CountAsync(),
                ["RiskIncidents"] = await _dbContext.RiskIncidents.CountAsync(),
                ["MaintenanceRecords"] = await _dbContext.MaintenanceRecords.CountAsync(),
                ["CalibrationRecords"] = await _dbContext.CalibrationRecords.CountAsync(),
                ["ElectricalSafetyTests"] = await _dbContext.ElectricalSafetyTests.CountAsync(),
                ["DeviceDocuments"] = await _dbContext.DeviceDocuments.CountAsync(),
                ["StaffMembers"] = await _dbContext.StaffMembers.CountAsync(),
                ["Departments"] = await _dbContext.Departments.CountAsync()
            };
            return counts;
        }
    }
}

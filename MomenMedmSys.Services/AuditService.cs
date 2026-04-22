using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    public class AuditService : IAuditService
    {
        private readonly MedMsysDbContext _dbContext;

        public AuditService(MedMsysDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogAsync(string entityType, int entityId, string action, int? userId, string userName,
            string? oldValues = null, string? newValues = null, string? affectedRecords = null, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                UserId = userId,
                UserName = userName,
                OldValues = oldValues,
                NewValues = newValues,
                AffectedRecords = affectedRecords,
                IpAddress = ipAddress ?? "Local",
                Timestamp = DateTime.Now,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _dbContext.AuditLogs.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string? entityType = null, int? entityId = null,
            DateTime? startDate = null, DateTime? endDate = null, int? userId = null)
        {
            var query = _dbContext.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Timestamp <= endDate.Value);

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityType, int entityId)
        {
            return await _dbContext.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.AuditLogs
                .Where(a => a.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Timestamp <= endDate.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentActivityAsync(int count = 50)
        {
            return await _dbContext.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task ExportAuditLogsAsync(string filePath, string? entityType = null, int? entityId = null,
            DateTime? startDate = null, DateTime? endDate = null, int? userId = null)
        {
            var logs = await GetAuditLogsAsync(entityType, entityId, startDate, endDate, userId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Audit Logs");

            // Header row
            var headers = new[]
            {
                "Timestamp", "Entity Type", "Entity ID", "Action", "User",
                "Affected Records", "IP Address", "Old Values", "New Values"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Data rows
            int row = 2;
            foreach (var log in logs)
            {
                worksheet.Cell(row, 1).Value = log.Timestamp;
                worksheet.Cell(row, 1).Style.NumberFormat.Format = "yyyy-MM-dd HH:mm:ss";

                worksheet.Cell(row, 2).Value = log.EntityType;
                worksheet.Cell(row, 3).Value = log.EntityId;
                worksheet.Cell(row, 4).Value = log.Action;
                worksheet.Cell(row, 5).Value = log.UserName;
                worksheet.Cell(row, 6).Value = log.AffectedRecords ?? string.Empty;
                worksheet.Cell(row, 7).Value = log.IpAddress;
                worksheet.Cell(row, 8).Value = log.OldValues ?? string.Empty;
                worksheet.Cell(row, 9).Value = log.NewValues ?? string.Empty;

                // Alternate row shading
                if (row % 2 == 0)
                {
                    for (int i = 1; i <= headers.Length; i++)
                    {
                        worksheet.Cell(row, i).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                    }
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Freeze header row
            worksheet.SheetView.FreezeRows(1);

            // Auto-filter
            worksheet.Range(1, 1, row - 1, headers.Length).SetAutoFilter();

            workbook.SaveAs(filePath);
        }

        public async Task<int> GetTotalAuditLogCountAsync()
        {
            return await _dbContext.AuditLogs.CountAsync();
        }
    }
}

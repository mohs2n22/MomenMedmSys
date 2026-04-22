using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Core.Enums;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for in-app notification management — notification CRUD, read/unread tracking, per-user queries,
    /// bulk mark-as-read, and system alert generation based on maintenance/calibration/risk thresholds.
    /// </summary>
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<int> GetUnreadCountAsync(int? userId = null);
        Task<IEnumerable<Notification>> GetNotificationsAsync(int? userId = null, bool? isRead = null, int count = 50);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<int> MarkAllAsReadAsync(int? userId = null);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task<int> GenerateSystemAlertsAsync();
        Task<Dictionary<string, int>> GetNotificationsSummaryAsync(int? userId = null);
    }

    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.Now;
            notification.IsActive = true;
            notification.IsRead = false;
            notification.CreatedBy = "System";

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            return notification;
        }

        public async Task<int> GetUnreadCountAsync(int? userId = null)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(n =>
                n.IsActive && !n.IsRead && (userId == null || n.UserId == null || n.UserId == userId));
            return notifications.Count();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync(int? userId = null, bool? isRead = null, int count = 50)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(n => n.IsActive);

            if (userId != null)
                notifications = notifications.Where(n => n.UserId == null || n.UserId == userId);

            if (isRead.HasValue)
                notifications = notifications.Where(n => n.IsRead == isRead.Value);

            return notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToList();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null || notification.IsRead)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;
            notification.UpdatedAt = DateTime.Now;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<int> MarkAllAsReadAsync(int? userId = null)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(n =>
                n.IsActive && !n.IsRead && (userId == null || n.UserId == null || n.UserId == userId));

            var now = DateTime.Now;
            int count = 0;
            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.ReadAt = now;
                n.UpdatedAt = now;
                _unitOfWork.Notifications.Update(n);
                count++;
            }

            if (count > 0)
                await _unitOfWork.SaveChangesAsync();

            return count;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
                return false;

            _unitOfWork.Notifications.Remove(notification);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, int>> GetNotificationsSummaryAsync(int? userId = null)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(n => n.IsActive && !n.IsRead);

            if (userId != null)
                notifications = notifications.Where(n => n.UserId == null || n.UserId == userId);

            var summary = new Dictionary<string, int>();

            // By type
            foreach (NotificationType type in Enum.GetValues(typeof(NotificationType)))
            {
                summary[$"Type_{type}"] = notifications.Count(n => n.Type == type);
            }

            // By priority
            foreach (NotificationPriority priority in Enum.GetValues(typeof(NotificationPriority)))
            {
                summary[$"Priority_{priority}"] = notifications.Count(n => n.Priority == priority);
            }

            summary["Total_Unread"] = notifications.Count();

            return summary;
        }

        /// <summary>
        /// Scans the system for conditions that require alerts and generates notifications.
        /// Should be called on app startup and periodically (e.g., every 30 minutes).
        /// </summary>
        public async Task<int> GenerateSystemAlertsAsync()
        {
            int created = 0;
            var now = DateTime.Now;

            // 1. Overdue maintenance
            try
            {
                var overdueMaintenance = await _unitOfWork.MaintenanceRecords.FindAsync(m =>
                    m.Status == MaintenanceStatus.Scheduled && m.ScheduledDate < now);

                foreach (var record in overdueMaintenance.Take(20)) // Limit to avoid spam
                {
                    var daysOverdue = (int)(now - record.ScheduledDate).TotalDays;
                    var priority = daysOverdue > 7 ? NotificationPriority.Critical :
                                   daysOverdue > 3 ? NotificationPriority.High : NotificationPriority.Medium;

                    var exists = await NotificationExistsAsync("MaintenanceRecord", record.Id, NotificationType.Maintenance);
                    if (!exists)
                    {
                        await CreateNotificationAsync(new Notification
                        {
                            Title = $"Overdue Maintenance: {record.Title}",
                            Message = $"Maintenance for device '{record.Device?.DeviceName}' is {daysOverdue} days overdue. Status: {record.Status}",
                            Type = NotificationType.Maintenance,
                            Priority = priority,
                            EntityType = "MaintenanceRecord",
                            EntityId = record.Id,
                            ActionUrl = $"maintenance/edit/{record.Id}",
                            DueDate = record.ScheduledDate,
                            CreatedBy = "System"
                        });
                        created++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error generating overdue maintenance alerts: {ex.Message}");
            }

            // 2. Calibration due/overdue
            try
            {
                var calibrationRecords = await _unitOfWork.CalibrationRecords.FindAsync(c => c.NextDueDate <= now.AddDays(30));

                foreach (var record in calibrationRecords.Take(20))
                {
                    var daysUntilDue = (int)(record.NextDueDate - now).TotalDays;
                    var isOverdue = daysUntilDue < 0;
                    var priority = isOverdue ? NotificationPriority.Critical :
                                   daysUntilDue <= 7 ? NotificationPriority.High : NotificationPriority.Medium;

                    var exists = await NotificationExistsAsync("CalibrationRecord", record.Id, NotificationType.Calibration);
                    if (!exists)
                    {
                        await CreateNotificationAsync(new Notification
                        {
                            Title = isOverdue ? $"Overdue Calibration: {record.Device?.DeviceName}" :
                                               $"Calibration Due Soon: {record.Device?.DeviceName}",
                            Message = isOverdue ? $"Calibration for '{record.Device?.DeviceName}' was due {Math.Abs(daysUntilDue)} days ago." :
                                                   $"Calibration for '{record.Device?.DeviceName}' is due in {daysUntilDue} days.",
                            Type = NotificationType.Calibration,
                            Priority = priority,
                            EntityType = "CalibrationRecord",
                            EntityId = record.Id,
                            ActionUrl = $"calibration/edit/{record.Id}",
                            DueDate = record.NextDueDate,
                            CreatedBy = "System"
                        });
                        created++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error generating calibration alerts: {ex.Message}");
            }

            // 3. Warranty expiring (30/60/90 days)
            try
            {
                var devices = await _unitOfWork.MedicalDevices.FindAsync(d =>
                    d.IsActive && d.Status != DeviceStatus.Disposed && d.Status != DeviceStatus.Decommissioned);

                foreach (var device in devices)
                {
                    if (device.WarrantyExpiryDate == default) continue;

                    var daysUntilExpiry = (int)(device.WarrantyExpiryDate - now).TotalDays;

                    // Only alert for 30, 60, 90 day thresholds
                    if (daysUntilExpiry > 90 || daysUntilExpiry < -30) continue;

                    var priority = daysUntilExpiry <= 30 ? NotificationPriority.High :
                                   daysUntilExpiry <= 60 ? NotificationPriority.Medium : NotificationPriority.Low;

                    var exists = await NotificationExistsAsync("MedicalDevice", device.Id, NotificationType.Warranty);
                    if (!exists)
                    {
                        await CreateNotificationAsync(new Notification
                        {
                            Title = daysUntilExpiry < 0 ? $"Warranty Expired: {device.DeviceName}" :
                                                          $"Warranty Expiring: {device.DeviceName}",
                            Message = daysUntilExpiry < 0 ? $"Warranty for '{device.DeviceName}' expired {Math.Abs(daysUntilExpiry)} days ago." :
                                                            $"Warranty for '{device.DeviceName}' expires in {daysUntilExpiry} days ({device.WarrantyExpiryDate:d}).",
                            Type = NotificationType.Warranty,
                            Priority = priority,
                            EntityType = "MedicalDevice",
                            EntityId = device.Id,
                            ActionUrl = $"device/edit/{device.Id}",
                            DueDate = device.WarrantyExpiryDate,
                            CreatedBy = "System"
                        });
                        created++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error generating warranty alerts: {ex.Message}");
            }

            // 4. Low stock parts
            try
            {
                var lowStockParts = await _unitOfWork.SpareParts.FindAsync(p => p.IsActive && p.IsLowStock);

                foreach (var part in lowStockParts.Take(20))
                {
                    var priority = part.CurrentStock == 0 ? NotificationPriority.Critical :
                                   part.IsCritical ? NotificationPriority.High : NotificationPriority.Medium;

                    var exists = await NotificationExistsAsync("SparePart", part.Id, NotificationType.Stock);
                    if (!exists)
                    {
                        await CreateNotificationAsync(new Notification
                        {
                            Title = part.CurrentStock == 0 ? $"OUT OF STOCK: {part.PartName}" :
                                                           $"Low Stock: {part.PartName}",
                            Message = part.CurrentStock == 0 ? $"'{part.PartName}' (P/N: {part.PartNumber}) is out of stock. Reorder point: {part.ReorderPoint}." :
                                                             $"'{part.PartName}' (P/N: {part.PartNumber}) is low on stock: {part.CurrentStock} remaining (min: {part.MinimumStock}).",
                            Type = NotificationType.Stock,
                            Priority = priority,
                            EntityType = "SparePart",
                            EntityId = part.Id,
                            ActionUrl = $"spareparts/edit/{part.Id}",
                            DueDate = now.AddDays(part.LeadTimeDays > 0 ? part.LeadTimeDays : 7),
                            CreatedBy = "System"
                        });
                        created++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error generating low stock alerts: {ex.Message}");
            }

            // 5. Critical risk incidents
            try
            {
                var criticalIncidents = await _unitOfWork.RiskIncidents.FindAsync(r =>
                    r.IsActive && (r.Severity == SeverityLevel.Critical || r.Severity == SeverityLevel.Major) &&
                    (r.Status == IncidentStatus.Open || r.Status == IncidentStatus.UnderInvestigation));

                foreach (var incident in criticalIncidents.Take(10))
                {
                    var priority = incident.Severity == SeverityLevel.Critical ? NotificationPriority.Critical : NotificationPriority.High;

                    var exists = await NotificationExistsAsync("RiskIncident", incident.Id, NotificationType.Risk);
                    if (!exists)
                    {
                        await CreateNotificationAsync(new Notification
                        {
                            Title = $"Critical Risk Incident: {incident.Title}",
                            Message = $"Risk incident '{incident.Title}' (Code: {incident.IncidentCode}) on device '{incident.Device?.DeviceName}' is {incident.Status}. Severity: {incident.Severity}.",
                            Type = NotificationType.Risk,
                            Priority = priority,
                            EntityType = "RiskIncident",
                            EntityId = incident.Id,
                            ActionUrl = $"risk/edit/{incident.Id}",
                            DueDate = incident.ActionDeadline ?? now.AddDays(1),
                            CreatedBy = "System"
                        });
                        created++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error generating risk incident alerts: {ex.Message}");
            }

            // 6. Overdue work orders / SLA breached
            try
            {
                var workOrders = await _unitOfWork.WorkOrders.FindAsync(w =>
                    w.IsActive && (w.Status == WorkOrderStatus.Open || w.Status == WorkOrderStatus.Assigned || w.Status == WorkOrderStatus.InProgress));

                foreach (var wo in workOrders.Take(20))
                {
                    bool isSlaBreached = wo.SLADeadline.HasValue && now > wo.SLADeadline.Value;
                    var daysOverdue = wo.SLADeadline.HasValue ? (int)(now - wo.SLADeadline.Value).TotalDays : 0;

                    if (!isSlaBreached && wo.Priority < WorkOrderPriority.High) continue;

                    var priority = isSlaBreached ? NotificationPriority.Critical :
                                   wo.Priority >= WorkOrderPriority.Critical ? NotificationPriority.Critical :
                                   wo.Priority >= WorkOrderPriority.High ? NotificationPriority.High : NotificationPriority.Medium;

                    var exists = await NotificationExistsAsync("WorkOrder", wo.Id, NotificationType.System);
                    if (!exists)
                    {
                        await CreateNotificationAsync(new Notification
                        {
                            Title = isSlaBreached ? $"SLA Breached: Work Order {wo.WorkOrderNumber}" :
                                                    $"High Priority Work Order: {wo.WorkOrderNumber}",
                            Message = isSlaBreached ? $"Work order '{wo.WorkOrderNumber}' for '{wo.Device?.DeviceName}' has breached SLA by {daysOverdue} days." :
                                                      $"Work order '{wo.WorkOrderNumber}' for '{wo.Device?.DeviceName}' is high priority. Status: {wo.Status}.",
                            Type = NotificationType.System,
                            Priority = priority,
                            EntityType = "WorkOrder",
                            EntityId = wo.Id,
                            ActionUrl = $"workorders/edit/{wo.Id}",
                            DueDate = wo.SLADeadline ?? wo.ScheduledEndDate ?? now.AddDays(1),
                            CreatedBy = "System"
                        });
                        created++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationService] Error generating work order alerts: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine($"[NotificationService] Generated {created} new alerts at {now:yyyy-MM-dd HH:mm}");
            return created;
        }

        private async Task<bool> NotificationExistsAsync(string entityType, int entityId, NotificationType type)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(n =>
                n.IsActive && n.EntityType == entityType && n.EntityId == entityId && n.Type == type && !n.IsRead);
            return notifications.Any();
        }
    }
}

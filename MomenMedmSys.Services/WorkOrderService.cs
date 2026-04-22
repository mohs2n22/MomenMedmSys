using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for work order lifecycle management — creation, assignment, tracking, priority/status filtering,
    /// overdue detection, and auto-generated work order numbering.
    /// </summary>
    public interface IWorkOrderService
    {
        Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync();
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByDeviceIdAsync(int deviceId);
        Task<WorkOrder?> GetWorkOrderByIdAsync(int id);
        Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder);
        Task UpdateWorkOrderAsync(WorkOrder workOrder);
        Task DeleteWorkOrderAsync(int id);
        Task<IEnumerable<WorkOrder>> GetOpenWorkOrdersAsync();
        Task<IEnumerable<WorkOrder>> GetOverdueWorkOrdersAsync();
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByStatusAsync(WorkOrderStatus status);
        Task<IEnumerable<WorkOrder>> GetWorkOrdersByPriorityAsync(WorkOrderPriority priority);
        Task<int> GetOpenWorkOrderCountAsync();
        Task<int> GetOverdueWorkOrderCountAsync();
        Task<string> GenerateWorkOrderNumberAsync();
    }

    public class WorkOrderService : IWorkOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private int _lastNumber = 0;

        public WorkOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync()
        {
            return await _unitOfWork.WorkOrders.GetAllAsync();
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrdersByDeviceIdAsync(int deviceId)
        {
            return await _unitOfWork.WorkOrders.FindAsync(w => w.DeviceId == deviceId);
        }

        public async Task<WorkOrder?> GetWorkOrderByIdAsync(int id)
        {
            return await _unitOfWork.WorkOrders.GetByIdAsync(id);
        }

        public async Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder)
        {
            workOrder.WorkOrderNumber = await GenerateWorkOrderNumberAsync();
            workOrder.ReportDate = DateTime.Now;
            workOrder.Status = WorkOrderStatus.Open;
            await _unitOfWork.WorkOrders.AddAsync(workOrder);
            await _unitOfWork.SaveChangesAsync();
            return workOrder;
        }

        public async Task UpdateWorkOrderAsync(WorkOrder workOrder)
        {
            workOrder.UpdatedAt = DateTime.Now;

            // Calculate response and resolution times
            if (workOrder.Status == WorkOrderStatus.Assigned && workOrder.AssignedDate.HasValue)
            {
                workOrder.ResponseTimeHours = (int)(workOrder.AssignedDate.Value - workOrder.ReportDate).TotalHours;
            }

            if (workOrder.Status == WorkOrderStatus.Completed && workOrder.CompletedDate.HasValue)
            {
                workOrder.ResolutionTimeHours = (int)(workOrder.CompletedDate.Value - workOrder.ReportDate).TotalHours;
            }

            _unitOfWork.WorkOrders.Update(workOrder);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteWorkOrderAsync(int id)
        {
            var workOrder = await _unitOfWork.WorkOrders.GetByIdAsync(id);
            if (workOrder != null)
            {
                _unitOfWork.WorkOrders.Remove(workOrder);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<WorkOrder>> GetOpenWorkOrdersAsync()
        {
            return await _unitOfWork.WorkOrders.FindAsync(w =>
                w.Status == WorkOrderStatus.Open ||
                w.Status == WorkOrderStatus.Assigned ||
                w.Status == WorkOrderStatus.InProgress);
        }

        public async Task<IEnumerable<WorkOrder>> GetOverdueWorkOrdersAsync()
        {
            var now = DateTime.Now;
            return await _unitOfWork.WorkOrders.FindAsync(w =>
                w.SLADeadline.HasValue && w.SLADeadline.Value < now &&
                (w.Status == WorkOrderStatus.Open ||
                 w.Status == WorkOrderStatus.Assigned ||
                 w.Status == WorkOrderStatus.InProgress));
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrdersByStatusAsync(WorkOrderStatus status)
        {
            return await _unitOfWork.WorkOrders.FindAsync(w => w.Status == status);
        }

        public async Task<IEnumerable<WorkOrder>> GetWorkOrdersByPriorityAsync(WorkOrderPriority priority)
        {
            return await _unitOfWork.WorkOrders.FindAsync(w => w.Priority == priority);
        }

        public async Task<int> GetOpenWorkOrderCountAsync()
        {
            var open = await GetOpenWorkOrdersAsync();
            return open.Count();
        }

        public async Task<int> GetOverdueWorkOrderCountAsync()
        {
            var overdue = await GetOverdueWorkOrdersAsync();
            return overdue.Count();
        }

        public async Task<string> GenerateWorkOrderNumberAsync()
        {
            var allOrders = await _unitOfWork.WorkOrders.GetAllAsync();
            _lastNumber = allOrders.Any() ? allOrders.Max(w => w.Id) + 1 : 1001;
            return $"WO-{DateTime.Now:yyyy}-{_lastNumber:D4}";
        }
    }
}

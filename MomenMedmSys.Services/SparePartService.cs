using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for spare parts inventory management — CRUD operations, low-stock/critical part detection,
    /// usage history tracking, inventory valuation, and part consumption recording.
    /// </summary>
    public interface ISparePartService
    {
        Task<IEnumerable<SparePart>> GetAllPartsAsync();
        Task<SparePart?> GetPartByIdAsync(int id);
        Task<SparePart> CreatePartAsync(SparePart part);
        Task UpdatePartAsync(SparePart part);
        Task DeletePartAsync(int id);
        Task<IEnumerable<SparePart>> GetLowStockPartsAsync();
        Task<IEnumerable<SparePart>> GetReorderNeededAsync();
        Task<IEnumerable<SparePart>> GetCriticalPartsAsync();
        Task<IEnumerable<SparePartUsage>> GetUsageHistoryAsync(int sparePartId);
        Task<decimal> GetTotalInventoryValueAsync();
        Task<int> GetLowStockCountAsync();
        Task<bool> UsePartAsync(int sparePartId, int maintenanceRecordId, int quantity, string notes);
    }

    public class SparePartService : ISparePartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SparePartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SparePart>> GetAllPartsAsync()
        {
            return await _unitOfWork.SpareParts.GetAllAsync();
        }

        public async Task<SparePart?> GetPartByIdAsync(int id)
        {
            return await _unitOfWork.SpareParts.GetByIdAsync(id);
        }

        public async Task<SparePart> CreatePartAsync(SparePart part)
        {
            part.CreatedAt = DateTime.Now;
            await _unitOfWork.SpareParts.AddAsync(part);
            await _unitOfWork.SaveChangesAsync();
            return part;
        }

        public async Task UpdatePartAsync(SparePart part)
        {
            part.UpdatedAt = DateTime.Now;
            _unitOfWork.SpareParts.Update(part);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePartAsync(int id)
        {
            var part = await _unitOfWork.SpareParts.GetByIdAsync(id);
            if (part != null)
            {
                _unitOfWork.SpareParts.Remove(part);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SparePart>> GetLowStockPartsAsync()
        {
            var all = await _unitOfWork.SpareParts.GetAllAsync();
            return all.Where(p => p.IsLowStock && p.IsActive);
        }

        public async Task<IEnumerable<SparePart>> GetReorderNeededAsync()
        {
            var all = await _unitOfWork.SpareParts.GetAllAsync();
            return all.Where(p => p.NeedsReorder && p.IsActive && !p.IsObsolete);
        }

        public async Task<IEnumerable<SparePart>> GetCriticalPartsAsync()
        {
            return await _unitOfWork.SpareParts.FindAsync(p => p.IsCritical && p.IsActive);
        }

        public async Task<IEnumerable<SparePartUsage>> GetUsageHistoryAsync(int sparePartId)
        {
            return await _unitOfWork.SparePartUsages.FindAsync(u => u.SparePartId == sparePartId);
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            var parts = await _unitOfWork.SpareParts.GetAllAsync();
            return parts.Where(p => p.IsActive).Sum(p => p.CurrentStock * p.UnitCost);
        }

        public async Task<int> GetLowStockCountAsync()
        {
            var lowStock = await GetLowStockPartsAsync();
            return lowStock.Count();
        }

        public async Task<bool> UsePartAsync(int sparePartId, int maintenanceRecordId, int quantity, string notes)
        {
            var part = await _unitOfWork.SpareParts.GetByIdAsync(sparePartId);
            if (part == null || part.CurrentStock < quantity)
                return false;

            // Deduct stock
            part.CurrentStock -= quantity;
            part.LastUsedDate = DateTime.Now;
            part.TotalUsageCount += quantity;
            _unitOfWork.SpareParts.Update(part);

            // Create usage record
            var usage = new SparePartUsage
            {
                SparePartId = sparePartId,
                MaintenanceRecordId = maintenanceRecordId,
                QuantityUsed = quantity,
                Notes = notes,
                UsedDate = DateTime.Now
            };
            await _unitOfWork.SparePartUsages.AddAsync(usage);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

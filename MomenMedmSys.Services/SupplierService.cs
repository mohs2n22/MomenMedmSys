using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for supplier/vendor management — CRUD operations, approved supplier queries,
    /// rating-based filtering, and performance tracking.
    /// </summary>
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<Supplier> CreateAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);
        Task<IEnumerable<Supplier>> GetApprovedAsync();
        Task<IEnumerable<Supplier>> GetByRatingAsync(int minRating);
        Task<int> GetApprovedCountAsync();
    }

    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _unitOfWork.Suppliers.GetAllAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Suppliers.GetByIdAsync(id);
        }

        public async Task<Supplier> CreateAsync(Supplier supplier)
        {
            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
            return supplier;
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            supplier.UpdatedAt = DateTime.Now;
            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier != null)
            {
                _unitOfWork.Suppliers.Remove(supplier);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Supplier>> GetApprovedAsync()
        {
            return await _unitOfWork.Suppliers.FindAsync(s => s.IsApproved);
        }

        public async Task<IEnumerable<Supplier>> GetByRatingAsync(int minRating)
        {
            return await _unitOfWork.Suppliers.FindAsync(s => s.Rating >= minRating);
        }

        public async Task<int> GetApprovedCountAsync()
        {
            var approved = await GetApprovedAsync();
            return approved.Count();
        }
    }
}

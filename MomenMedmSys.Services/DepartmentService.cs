using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for department CRUD operations — manage hospital departments and associate them with
    /// devices, staff, and work orders.
    /// </summary>
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<Department> CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
        Task<IEnumerable<Department>> GetActiveAsync();
        Task<int> GetActiveCountAsync();
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _unitOfWork.Departments.GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Departments.GetByIdAsync(id);
        }

        public async Task<Department> CreateAsync(Department department)
        {
            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();
            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            department.UpdatedAt = DateTime.Now;
            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dept = await _unitOfWork.Departments.GetByIdAsync(id);
            if (dept != null)
            {
                _unitOfWork.Departments.Remove(dept);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Department>> GetActiveAsync()
        {
            return await _unitOfWork.Departments.FindAsync(d => d.IsActive);
        }

        public async Task<int> GetActiveCountAsync()
        {
            var active = await GetActiveAsync();
            return active.Count();
        }
    }
}

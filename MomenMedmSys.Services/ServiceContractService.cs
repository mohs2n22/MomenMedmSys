using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for external service contract management — CRUD, active/expiring/expired contract queries,
    /// SLA tracking, and contract value aggregation.
    /// </summary>
    public interface IServiceContractService
    {
        Task<IEnumerable<ServiceContract>> GetAllContractsAsync();
        Task<ServiceContract?> GetContractByIdAsync(int id);
        Task<ServiceContract> CreateContractAsync(ServiceContract contract);
        Task UpdateContractAsync(ServiceContract contract);
        Task DeleteContractAsync(int id);
        Task<IEnumerable<ServiceContract>> GetActiveContractsAsync();
        Task<IEnumerable<ServiceContract>> GetExpiringSoonContractsAsync(int daysThreshold = 30);
        Task<IEnumerable<ServiceContract>> GetExpiredContractsAsync();
        Task<int> GetActiveContractCountAsync();
        Task<int> GetExpiringSoonCountAsync(int daysThreshold = 30);
        Task<decimal> GetTotalContractValueAsync();
    }

    public class ServiceContractService : IServiceContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ServiceContract>> GetAllContractsAsync()
        {
            return await _unitOfWork.ServiceContracts.GetAllAsync();
        }

        public async Task<ServiceContract?> GetContractByIdAsync(int id)
        {
            return await _unitOfWork.ServiceContracts.GetByIdAsync(id);
        }

        public async Task<ServiceContract> CreateContractAsync(ServiceContract contract)
        {
            await _unitOfWork.ServiceContracts.AddAsync(contract);
            await _unitOfWork.SaveChangesAsync();
            return contract;
        }

        public async Task UpdateContractAsync(ServiceContract contract)
        {
            contract.UpdatedAt = DateTime.Now;
            _unitOfWork.ServiceContracts.Update(contract);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteContractAsync(int id)
        {
            var contract = await _unitOfWork.ServiceContracts.GetByIdAsync(id);
            if (contract != null)
            {
                _unitOfWork.ServiceContracts.Remove(contract);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ServiceContract>> GetActiveContractsAsync()
        {
            return await _unitOfWork.ServiceContracts.FindAsync(c => c.Status == ContractStatus.Active);
        }

        public async Task<IEnumerable<ServiceContract>> GetExpiringSoonContractsAsync(int daysThreshold = 30)
        {
            var all = await _unitOfWork.ServiceContracts.GetAllAsync();
            return all.Where(c => c.EndDate <= DateTime.Now.AddDays(daysThreshold) && c.Status == ContractStatus.Active);
        }

        public async Task<IEnumerable<ServiceContract>> GetExpiredContractsAsync()
        {
            var all = await _unitOfWork.ServiceContracts.GetAllAsync();
            return all.Where(c => c.EndDate < DateTime.Now && c.Status != ContractStatus.Terminated);
        }

        public async Task<int> GetActiveContractCountAsync()
        {
            var active = await GetActiveContractsAsync();
            return active.Count();
        }

        public async Task<int> GetExpiringSoonCountAsync(int daysThreshold = 30)
        {
            var expiring = await GetExpiringSoonContractsAsync(daysThreshold);
            return expiring.Count();
        }

        public async Task<decimal> GetTotalContractValueAsync()
        {
            var all = await GetAllContractsAsync();
            return all.Where(c => c.Status == ContractStatus.Active).Sum(c => c.ContractValue);
        }
    }
}

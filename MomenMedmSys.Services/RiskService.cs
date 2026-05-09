using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for risk incident management per ISO 14971 — incident CRUD, severity/probability scoring,
    /// risk level classification, open/critical incident tracking.
    /// </summary>
    public interface IRiskService
    {
        Task<IEnumerable<RiskIncident>> GetAllIncidentsAsync();
        Task<IEnumerable<RiskIncident>> GetIncidentsByDeviceIdAsync(int deviceId);
        Task<RiskIncident?> GetIncidentByIdAsync(int id);
        Task<RiskIncident> CreateIncidentAsync(RiskIncident incident);
        Task UpdateIncidentAsync(RiskIncident incident);
        Task DeleteIncidentAsync(int id);
        Task<IEnumerable<RiskIncident>> GetOpenIncidentsAsync();
        Task<IEnumerable<RiskIncident>> GetIncidentsByRiskLevelAsync(RiskLevel riskLevel);
        Task<int> GetOpenIncidentCountAsync();
        Task<int> GetCriticalIncidentCountAsync();
    }

    public class RiskService : IRiskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RiskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RiskIncident>> GetAllIncidentsAsync()
        {
            return await _unitOfWork.RiskIncidents.GetAllAsync();
        }

        public async Task<IEnumerable<RiskIncident>> GetIncidentsByDeviceIdAsync(int deviceId)
        {
            return await _unitOfWork.RiskIncidents.FindAsync(i => i.DeviceId == deviceId);
        }

        public async Task<RiskIncident?> GetIncidentByIdAsync(int id)
        {
            return await _unitOfWork.RiskIncidents.GetByIdAsync(id);
        }

        public async Task<RiskIncident> CreateIncidentAsync(RiskIncident incident)
        {
            incident.CreatedAt = DateTime.Now;
            await _unitOfWork.RiskIncidents.AddAsync(incident);
            await _unitOfWork.SaveChangesAsync();
            return incident;
        }

        public async Task UpdateIncidentAsync(RiskIncident incident)
        {
            incident.UpdatedAt = DateTime.Now;
            _unitOfWork.RiskIncidents.Update(incident);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteIncidentAsync(int id)
        {
            var incident = await _unitOfWork.RiskIncidents.GetByIdAsync(id);
            if (incident != null)
            {
                _unitOfWork.RiskIncidents.Remove(incident);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RiskIncident>> GetOpenIncidentsAsync()
        {
            return await _unitOfWork.RiskIncidents.FindAsync(i =>
                i.Status == IncidentStatus.Open ||
                i.Status == IncidentStatus.UnderInvestigation ||
                i.Status == IncidentStatus.PendingAction);
        }

        public async Task<IEnumerable<RiskIncident>> GetIncidentsByRiskLevelAsync(RiskLevel riskLevel)
        {
            // OverallRisk is computed, filter in-memory
            var all = await _unitOfWork.RiskIncidents.GetAllAsync();
            return all.Where(i => i.OverallRisk == riskLevel);
        }

        public async Task<int> GetOpenIncidentCountAsync()
        {
            var open = await GetOpenIncidentsAsync();
            return open.Count();
        }

        public async Task<int> GetCriticalIncidentCountAsync()
        {
            // OverallRisk is computed, filter in-memory
            var all = await _unitOfWork.RiskIncidents.GetAllAsync();
            return all.Count(i => i.OverallRisk == RiskLevel.Critical);
        }
    }
}

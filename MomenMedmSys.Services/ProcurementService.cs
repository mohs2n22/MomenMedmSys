using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for equipment procurement request management — request CRUD, status-based filtering,
    /// approval workflow tracking, technical evaluation management, and auto-generated request numbering.
    /// </summary>
    public interface IProcurementService
    {
        Task<IEnumerable<ProcurementRequest>> GetAllAsync();
        Task<ProcurementRequest?> GetByIdAsync(int id);
        Task<ProcurementRequest> CreateAsync(ProcurementRequest request);
        Task UpdateAsync(ProcurementRequest request);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProcurementRequest>> GetByStatusAsync(ProcurementStatus status);
        Task<IEnumerable<ProcurementRequest>> GetPendingAsync();
        Task<IEnumerable<ProcurementRequest>> GetApprovedAsync();
        Task<int> GetPendingCountAsync();
        Task<int> GetApprovedCountAsync();
        Task<string> GenerateRequestNumberAsync();

        // Technical Evaluation
        Task<IEnumerable<TechnicalEvaluation>> GetEvaluationsByRequestIdAsync(int requestId);
        Task<TechnicalEvaluation> CreateEvaluationAsync(TechnicalEvaluation evaluation);
        Task UpdateEvaluationAsync(TechnicalEvaluation evaluation);
        Task DeleteEvaluationAsync(int id);
    }

    public class ProcurementService : IProcurementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private int _lastNumber = 0;

        public ProcurementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProcurementRequest>> GetAllAsync()
        {
            return await _unitOfWork.ProcurementRequests.GetAllAsync();
        }

        public async Task<ProcurementRequest?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ProcurementRequests.GetByIdAsync(id);
        }

        public async Task<ProcurementRequest> CreateAsync(ProcurementRequest request)
        {
            request.RequestNumber = await GenerateRequestNumberAsync();
            request.RequestDate = DateTime.Now;
            await _unitOfWork.ProcurementRequests.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();
            return request;
        }

        public async Task UpdateAsync(ProcurementRequest request)
        {
            request.UpdatedAt = DateTime.Now;
            _unitOfWork.ProcurementRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var request = await _unitOfWork.ProcurementRequests.GetByIdAsync(id);
            if (request != null)
            {
                _unitOfWork.ProcurementRequests.Remove(request);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProcurementRequest>> GetByStatusAsync(ProcurementStatus status)
        {
            return await _unitOfWork.ProcurementRequests.FindAsync(p => p.Status == status);
        }

        public async Task<IEnumerable<ProcurementRequest>> GetPendingAsync()
        {
            return await _unitOfWork.ProcurementRequests.FindAsync(p =>
                p.Status == ProcurementStatus.Draft ||
                p.Status == ProcurementStatus.Submitted ||
                p.Status == ProcurementStatus.UnderReview);
        }

        public async Task<IEnumerable<ProcurementRequest>> GetApprovedAsync()
        {
            return await _unitOfWork.ProcurementRequests.FindAsync(p => p.Status == ProcurementStatus.Approved);
        }

        public async Task<int> GetPendingCountAsync()
        {
            var pending = await GetPendingAsync();
            return pending.Count();
        }

        public async Task<int> GetApprovedCountAsync()
        {
            var approved = await GetApprovedAsync();
            return approved.Count();
        }

        public async Task<string> GenerateRequestNumberAsync()
        {
            var all = await GetAllAsync();
            _lastNumber = all.Any() ? all.Max(p => p.Id) + 1 : 1001;
            return $"PR-{DateTime.Now:yyyy}-{_lastNumber:D4}";
        }

        // Technical Evaluation methods
        public async Task<IEnumerable<TechnicalEvaluation>> GetEvaluationsByRequestIdAsync(int requestId)
        {
            return await _unitOfWork.TechnicalEvaluations.FindAsync(e => e.ProcurementRequestId == requestId);
        }

        public async Task<TechnicalEvaluation> CreateEvaluationAsync(TechnicalEvaluation evaluation)
        {
            await _unitOfWork.TechnicalEvaluations.AddAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();
            return evaluation;
        }

        public async Task UpdateEvaluationAsync(TechnicalEvaluation evaluation)
        {
            evaluation.UpdatedAt = DateTime.Now;
            _unitOfWork.TechnicalEvaluations.Update(evaluation);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteEvaluationAsync(int id)
        {
            // Note: TechnicalEvaluation doesn't have a direct GetById in repository
            var all = await _unitOfWork.TechnicalEvaluations.GetAllAsync();
            var eval = all.FirstOrDefault(e => e.Id == id);
            if (eval != null)
            {
                _unitOfWork.TechnicalEvaluations.Remove(eval);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

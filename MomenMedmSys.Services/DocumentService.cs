using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    /// <summary>
    /// Service for device document management — manual, certificate, warranty, and specification storage
    /// with version control, per-device queries, and document statistics.
    /// </summary>
    public interface IDocumentService
    {
        Task<IEnumerable<DeviceDocument>> GetAllDocumentsAsync();
        Task<IEnumerable<DeviceDocument>> GetDocumentsByDeviceAsync(int deviceId);
        Task<DeviceDocument?> GetDocumentByIdAsync(int id);
        Task AddDocumentAsync(DeviceDocument document);
        Task UpdateDocumentAsync(DeviceDocument document);
        Task DeleteDocumentAsync(int id);
        Task<IEnumerable<DeviceDocument>> GetDocumentsByTypeAsync(DocumentType documentType);
        Task<DocumentStats> GetDeviceDocumentStatsAsync(int deviceId);
    }

    public class DocumentStats
    {
        public int TotalDocuments { get; set; }
        public long TotalSize { get; set; }
        public int Manuals { get; set; }
        public int Certificates { get; set; }
        public int Warranties { get; set; }
        public int Specifications { get; set; }
        public int TrainingMaterials { get; set; }
        public int RegulatoryCertificates { get; set; }
        public int CalibrationCertificates { get; set; }
        public int SafetyTestReports { get; set; }
        public int TechnicalDrawings { get; set; }
        public int SoftwareManuals { get; set; }
        public int Others { get; set; }
    }

    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DeviceDocument>> GetAllDocumentsAsync()
        {
            return await _unitOfWork.DeviceDocuments.GetAllAsync();
        }

        public async Task<IEnumerable<DeviceDocument>> GetDocumentsByDeviceAsync(int deviceId)
        {
            return await _unitOfWork.DeviceDocuments.FindAsync(d => d.DeviceId == deviceId);
        }

        public async Task<DeviceDocument?> GetDocumentByIdAsync(int id)
        {
            return await _unitOfWork.DeviceDocuments.GetByIdAsync(id);
        }

        public async Task AddDocumentAsync(DeviceDocument document)
        {
            document.UploadDate = DateTime.Now;
            await _unitOfWork.DeviceDocuments.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateDocumentAsync(DeviceDocument document)
        {
            _unitOfWork.DeviceDocuments.Update(document);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(int id)
        {
            var document = await _unitOfWork.DeviceDocuments.GetByIdAsync(id);
            if (document != null)
            {
                _unitOfWork.DeviceDocuments.Remove(document);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DeviceDocument>> GetDocumentsByTypeAsync(DocumentType documentType)
        {
            return await _unitOfWork.DeviceDocuments.FindAsync(d => d.DocumentType == documentType);
        }

        public async Task<DocumentStats> GetDeviceDocumentStatsAsync(int deviceId)
        {
            var documents = await _unitOfWork.DeviceDocuments.FindAsync(d => d.DeviceId == deviceId);
            var docList = documents.ToList();

            var stats = new DocumentStats
            {
                TotalDocuments = docList.Count,
                TotalSize = docList.Sum(d => d.FileSize),
                Manuals = docList.Count(d => d.DocumentType == DocumentType.OperationManual || d.DocumentType == DocumentType.MaintenanceManual || d.DocumentType == DocumentType.SoftwareManual),
                Certificates = docList.Count(d => d.DocumentType == DocumentType.RegulatoryCertificate),
                Warranties = docList.Count(d => d.DocumentType == DocumentType.WarrantyCertificate),
                Specifications = docList.Count(d => d.DocumentType == DocumentType.TechnicalDrawing),
                TrainingMaterials = docList.Count(d => d.DocumentType == DocumentType.TrainingMaterial),
                RegulatoryCertificates = docList.Count(d => d.DocumentType == DocumentType.RegulatoryCertificate),
                CalibrationCertificates = docList.Count(d => d.DocumentType == DocumentType.CalibrationCertificate),
                SafetyTestReports = docList.Count(d => d.DocumentType == DocumentType.SafetyTestReport),
                TechnicalDrawings = docList.Count(d => d.DocumentType == DocumentType.TechnicalDrawing),
                SoftwareManuals = docList.Count(d => d.DocumentType == DocumentType.SoftwareManual),
                Others = docList.Count(d => d.DocumentType == DocumentType.Other)
            };

            return stats;
        }
    }
}

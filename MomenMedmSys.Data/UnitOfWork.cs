using System;
using System.Threading.Tasks;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MomenMedmSys.Data
{
    public interface IUnitOfWork : IDisposable
    {
        // Original entities
        IRepository<MedicalDevice> MedicalDevices { get; }
        IRepository<MaintenanceRecord> MaintenanceRecords { get; }
        IRepository<CalibrationRecord> CalibrationRecords { get; }
        IRepository<RiskIncident> RiskIncidents { get; }
        IRepository<SparePart> SpareParts { get; }
        IRepository<SparePartUsage> SparePartUsages { get; }
        IRepository<ServiceContract> ServiceContracts { get; }

        // New entities
        IRepository<Department> Departments { get; }
        IRepository<Supplier> Suppliers { get; }
        IRepository<StaffMember> StaffMembers { get; }
        IRepository<TrainingRecord> TrainingRecords { get; }
        IRepository<WorkOrder> WorkOrders { get; }
        IRepository<ProcurementRequest> ProcurementRequests { get; }
        IRepository<TechnicalEvaluation> TechnicalEvaluations { get; }
        IRepository<DeviceDocument> DeviceDocuments { get; }
        IRepository<ElectricalSafetyTest> ElectricalSafetyTests { get; }
        IRepository<NetworkDevice> NetworkDevices { get; }
        IRepository<DeviceActionLog> DeviceActionLogs { get; }
        IRepository<AssignedDevice> AssignedDevices { get; }

        // Auth & User Management
        IRepository<User> Users { get; }
        IRepository<UserSession> UserSessions { get; }

        // Audit & Notifications
        IRepository<AuditLog> AuditLogs { get; }
        IRepository<Notification> Notifications { get; }

        // Licensing
        IRepository<LicenseInfo> Licenses { get; }
        IRepository<LicenseDevice> LicensedDevices { get; }
        IRepository<HospitalSettings> HospitalSettings { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedMsysDbContext _context;
        private bool _disposed;

        // Original entities
        public IRepository<MedicalDevice> MedicalDevices { get; private set; }
        public IRepository<MaintenanceRecord> MaintenanceRecords { get; private set; }
        public IRepository<CalibrationRecord> CalibrationRecords { get; private set; }
        public IRepository<RiskIncident> RiskIncidents { get; private set; }
        public IRepository<SparePart> SpareParts { get; private set; }
        public IRepository<SparePartUsage> SparePartUsages { get; private set; }
        public IRepository<ServiceContract> ServiceContracts { get; private set; }

        // New entities
        public IRepository<Department> Departments { get; private set; }
        public IRepository<Supplier> Suppliers { get; private set; }
        public IRepository<StaffMember> StaffMembers { get; private set; }
        public IRepository<TrainingRecord> TrainingRecords { get; private set; }
        public IRepository<WorkOrder> WorkOrders { get; private set; }
        public IRepository<ProcurementRequest> ProcurementRequests { get; private set; }
        public IRepository<TechnicalEvaluation> TechnicalEvaluations { get; private set; }
        public IRepository<DeviceDocument> DeviceDocuments { get; private set; }
        public IRepository<ElectricalSafetyTest> ElectricalSafetyTests { get; private set; }

        public IRepository<NetworkDevice> NetworkDevices { get; private set; }
        public IRepository<DeviceActionLog> DeviceActionLogs { get; private set; }
        public IRepository<AssignedDevice> AssignedDevices { get; private set; }

        public IRepository<User> Users { get; private set; }
        public IRepository<UserSession> UserSessions { get; private set; }
        public IRepository<AuditLog> AuditLogs { get; private set; }
        public IRepository<Notification> Notifications { get; private set; }
        public IRepository<LicenseInfo> Licenses { get; private set; }
        public IRepository<LicenseDevice> LicensedDevices { get; private set; }
        public IRepository<HospitalSettings> HospitalSettings { get; private set; }

        public UnitOfWork(MedMsysDbContext context)
        {
            _context = context;
            MedicalDevices = new Repository<MedicalDevice>(_context);
            MaintenanceRecords = new Repository<MaintenanceRecord>(_context);
            CalibrationRecords = new Repository<CalibrationRecord>(_context);
            RiskIncidents = new Repository<RiskIncident>(_context);
            SpareParts = new Repository<SparePart>(_context);
            SparePartUsages = new Repository<SparePartUsage>(_context);
            ServiceContracts = new Repository<ServiceContract>(_context);
            Departments = new Repository<Department>(_context);
            Suppliers = new Repository<Supplier>(_context);
            StaffMembers = new Repository<StaffMember>(_context);
            TrainingRecords = new Repository<TrainingRecord>(_context);
            WorkOrders = new Repository<WorkOrder>(_context);
            ProcurementRequests = new Repository<ProcurementRequest>(_context);
            TechnicalEvaluations = new Repository<TechnicalEvaluation>(_context);
            DeviceDocuments = new Repository<DeviceDocument>(_context);
            ElectricalSafetyTests = new Repository<ElectricalSafetyTest>(_context);
            NetworkDevices = new Repository<NetworkDevice>(_context);
            DeviceActionLogs = new Repository<DeviceActionLog>(_context);
            AssignedDevices = new Repository<AssignedDevice>(_context);
            Users = new Repository<User>(_context);
            UserSessions = new Repository<UserSession>(_context);
            AuditLogs = new Repository<AuditLog>(_context);
            Notifications = new Repository<Notification>(_context);
            Licenses = new Repository<LicenseInfo>(_context);
            LicensedDevices = new Repository<LicenseDevice>(_context);
            HospitalSettings = new Repository<HospitalSettings>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}

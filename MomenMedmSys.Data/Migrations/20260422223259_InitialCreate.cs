using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomenMedmSys.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    OldValues = table.Column<string>(type: "TEXT", nullable: true),
                    NewValues = table.Column<string>(type: "TEXT", nullable: true),
                    AffectedRecords = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Manager = table.Column<string>(type: "TEXT", nullable: false),
                    Building = table.Column<string>(type: "TEXT", nullable: false),
                    Floor = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", nullable: false),
                    Budget = table.Column<decimal>(type: "TEXT", nullable: false),
                    DeviceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ActiveDeviceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicenseKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LicenseType = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PrimaryMacAddress = table.Column<string>(type: "TEXT", maxLength: 17, nullable: false),
                    HardwareFingerprint = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    MaxDevices = table.Column<int>(type: "INTEGER", nullable: false),
                    RegisteredDeviceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActivated = table.Column<bool>(type: "INTEGER", nullable: false),
                    HospitalName = table.Column<string>(type: "TEXT", nullable: false),
                    AdministratorName = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplierCode = table.Column<string>(type: "TEXT", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Fax = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: false),
                    TaxNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ProductCategories = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentTerms = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    TotalOrders = table.Column<int>(type: "INTEGER", nullable: false),
                    OnTimeDeliveries = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PasswordExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    SubRole = table.Column<string>(type: "TEXT", nullable: false),
                    Specialization = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: false),
                    HireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    IsActiveAccount = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Certifications = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseNumber = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CanManageDevices = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageMaintenance = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageCalibration = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageSpareParts = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanViewReports = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageNetworkDevices = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageStaff = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanAccessAdminPanel = table.Column<bool>(type: "INTEGER", nullable: false),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffMembers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LicensedDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicenseInfoId = table.Column<int>(type: "INTEGER", nullable: false),
                    MacAddress = table.Column<string>(type: "TEXT", maxLength: 17, nullable: false),
                    HardwareFingerprint = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    MachineName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensedDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicensedDevices_Licenses_LicenseInfoId",
                        column: x => x.LicenseInfoId,
                        principalTable: "Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ContractName = table.Column<string>(type: "TEXT", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: true),
                    Provider = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPerson = table.Column<string>(type: "TEXT", nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    RenewalNoticeDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RenewalNoticeDays = table.Column<int>(type: "INTEGER", nullable: false),
                    CoverageDescription = table.Column<string>(type: "TEXT", nullable: false),
                    ContractValue = table.Column<decimal>(type: "REAL", nullable: false),
                    PaymentTerms = table.Column<string>(type: "TEXT", nullable: false),
                    CoveredDeviceCategories = table.Column<string>(type: "TEXT", nullable: false),
                    CoveredDeviceIds = table.Column<string>(type: "TEXT", nullable: false),
                    ResponseTimeHours = table.Column<int>(type: "INTEGER", nullable: false),
                    ResolutionTimeHours = table.Column<int>(type: "INTEGER", nullable: false),
                    SLADetails = table.Column<string>(type: "TEXT", nullable: false),
                    PenaltyClause = table.Column<string>(type: "TEXT", nullable: false),
                    TotalCalls = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedCalls = table.Column<int>(type: "INTEGER", nullable: false),
                    SatisfactionScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    ContractFilePath = table.Column<string>(type: "TEXT", nullable: false),
                    WarrantyCertificatePath = table.Column<string>(type: "TEXT", nullable: false),
                    Attachments = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceContracts_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    ActionUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceCode = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    SubCategory = table.Column<string>(type: "TEXT", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    RFIDTag = table.Column<string>(type: "TEXT", nullable: false),
                    AssetTagNumber = table.Column<string>(type: "TEXT", nullable: false),
                    UdiCode = table.Column<string>(type: "TEXT", nullable: false),
                    RegulatoryClass = table.Column<string>(type: "TEXT", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "REAL", nullable: false),
                    AcquisitionMethod = table.Column<string>(type: "TEXT", nullable: false),
                    InstallationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CommissioningDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatedLifespanYears = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpectedDisposalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DepreciationMethod = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentDepreciatedValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    TotalMaintenanceCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: true),
                    SupplierName = table.Column<string>(type: "TEXT", nullable: false),
                    WarrantyProvider = table.Column<string>(type: "TEXT", nullable: false),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WarrantyCertificatePath = table.Column<string>(type: "TEXT", nullable: false),
                    WarrantyTerms = table.Column<string>(type: "TEXT", nullable: false),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    Building = table.Column<string>(type: "TEXT", nullable: false),
                    Floor = table.Column<string>(type: "TEXT", nullable: false),
                    Room = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<decimal>(type: "TEXT", nullable: true),
                    Longitude = table.Column<decimal>(type: "TEXT", nullable: true),
                    AssignedStaffId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssignedTo = table.Column<string>(type: "TEXT", nullable: false),
                    TechnicalSpecs = table.Column<string>(type: "TEXT", nullable: false),
                    PowerRequirements = table.Column<string>(type: "TEXT", nullable: false),
                    SoftwareVersion = table.Column<string>(type: "TEXT", nullable: false),
                    NetworkConnected = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresCalibration = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresPreventiveMaintenance = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresElectricalSafetyTesting = table.Column<bool>(type: "INTEGER", nullable: false),
                    OperationalManualPath = table.Column<string>(type: "TEXT", nullable: false),
                    MaintenanceManualPath = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DisposalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisposalReason = table.Column<string>(type: "TEXT", nullable: false),
                    RiskClassification = table.Column<int>(type: "INTEGER", nullable: false),
                    SafetyNotes = table.Column<string>(type: "TEXT", nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastCalibrationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastSafetyTestDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalDevices_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MedicalDevices_StaffMembers_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MedicalDevices_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssignedDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StaffMemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssignmentNotes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignedDevices_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignedDevices_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    DocumentType = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploadedBy = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsCurrentVersion = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDocuments_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElectricalSafetyTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TestType = table.Column<int>(type: "INTEGER", nullable: false),
                    TestStandard = table.Column<string>(type: "TEXT", nullable: false),
                    TestEquipmentUsed = table.Column<string>(type: "TEXT", nullable: false),
                    TestEquipmentCalibration = table.Column<string>(type: "TEXT", nullable: false),
                    EarthResistanceMeasured = table.Column<decimal>(type: "TEXT", nullable: true),
                    EarthResistanceLimit = table.Column<decimal>(type: "TEXT", nullable: false),
                    LeakageCurrentMeasured = table.Column<decimal>(type: "TEXT", nullable: true),
                    LeakageCurrentLimit = table.Column<decimal>(type: "TEXT", nullable: false),
                    InsulationResistanceMeasured = table.Column<decimal>(type: "TEXT", nullable: true),
                    InsulationResistanceLimit = table.Column<decimal>(type: "TEXT", nullable: false),
                    TouchCurrentMeasured = table.Column<decimal>(type: "TEXT", nullable: true),
                    TouchCurrentLimit = table.Column<decimal>(type: "TEXT", nullable: false),
                    VisualInspectionPass = table.Column<bool>(type: "INTEGER", nullable: false),
                    VisualInspectionNotes = table.Column<string>(type: "TEXT", nullable: false),
                    OverallResult = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: false),
                    CertificateNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PerformedBy = table.Column<string>(type: "TEXT", nullable: false),
                    TechnicianSignature = table.Column<string>(type: "TEXT", nullable: false),
                    IsExternalTester = table.Column<bool>(type: "INTEGER", nullable: false),
                    TestingCompany = table.Column<string>(type: "TEXT", nullable: false),
                    Attachments = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectricalSafetyTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectricalSafetyTests_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Recurrence = table.Column<int>(type: "INTEGER", nullable: false),
                    RecurrenceInterval = table.Column<int>(type: "INTEGER", nullable: false),
                    PerformedBy = table.Column<string>(type: "TEXT", nullable: false),
                    PerformedByStaffId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsExternalContractor = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContractorName = table.Column<string>(type: "TEXT", nullable: false),
                    ContractReference = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Findings = table.Column<string>(type: "TEXT", nullable: false),
                    ActionsTaken = table.Column<string>(type: "TEXT", nullable: false),
                    Recommendations = table.Column<string>(type: "TEXT", nullable: false),
                    RootCause = table.Column<string>(type: "TEXT", nullable: false),
                    DowntimeHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    VerificationPerformed = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerifiedBy = table.Column<string>(type: "TEXT", nullable: false),
                    VerificationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WorkOrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedCompletionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LaborCost = table.Column<decimal>(type: "REAL", nullable: false),
                    PartsCost = table.Column<decimal>(type: "REAL", nullable: false),
                    Attachments = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRecords_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceRecords_StaffMembers_PerformedByStaffId",
                        column: x => x.PerformedByStaffId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NetworkDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MedicalDeviceId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    Hostname = table.Column<string>(type: "TEXT", maxLength: 253, nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    MacAddress = table.Column<string>(type: "TEXT", maxLength: 17, nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    SubnetMask = table.Column<string>(type: "TEXT", nullable: false),
                    Gateway = table.Column<string>(type: "TEXT", nullable: false),
                    DnsServer = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "TEXT", nullable: false),
                    SoftwareVersion = table.Column<string>(type: "TEXT", nullable: false),
                    OperatingSystem = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceType = table.Column<string>(type: "TEXT", nullable: false),
                    ConnectionStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FirstDiscovered = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResponseTimeMs = table.Column<int>(type: "INTEGER", nullable: false),
                    UptimeHours = table.Column<int>(type: "INTEGER", nullable: false),
                    LastErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    LastErrorTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RemoteManagementEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ManagementProtocol = table.Column<string>(type: "TEXT", nullable: false),
                    ManagementPort = table.Column<string>(type: "TEXT", nullable: false),
                    SupportsRemoteUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsRemoteDiagnostics = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsRemoteConfiguration = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportsRemoteReboot = table.Column<bool>(type: "INTEGER", nullable: false),
                    RemoteAccessUrl = table.Column<string>(type: "TEXT", nullable: false),
                    AuthenticationMethod = table.Column<string>(type: "TEXT", nullable: false),
                    SslEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CertificateExpiry = table.Column<string>(type: "TEXT", nullable: false),
                    FirewallEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    OpenPorts = table.Column<string>(type: "TEXT", nullable: false),
                    CpuUsage = table.Column<double>(type: "REAL", nullable: false),
                    MemoryUsage = table.Column<double>(type: "REAL", nullable: false),
                    DiskUsage = table.Column<double>(type: "REAL", nullable: false),
                    NetworkBandwidthUsage = table.Column<double>(type: "REAL", nullable: false),
                    ActiveConnections = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorLogs = table.Column<string>(type: "TEXT", nullable: false),
                    SystemHealthStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    DiscoveredVia = table.Column<int>(type: "INTEGER", nullable: false),
                    DiscoveryProtocol = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkDevices_MedicalDevices_MedicalDeviceId",
                        column: x => x.MedicalDeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProcurementRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RequestNumber = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedBy = table.Column<string>(type: "TEXT", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    Justification = table.Column<string>(type: "TEXT", nullable: false),
                    GapAnalysis = table.Column<string>(type: "TEXT", nullable: false),
                    ClinicalNeed = table.Column<string>(type: "TEXT", nullable: false),
                    EstimatedPatientVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipmentType = table.Column<string>(type: "TEXT", nullable: false),
                    TechnicalSpecifications = table.Column<string>(type: "TEXT", nullable: false),
                    MinimumRequirements = table.Column<string>(type: "TEXT", nullable: false),
                    PreferredBrands = table.Column<string>(type: "TEXT", nullable: false),
                    BudgetEstimate = table.Column<decimal>(type: "REAL", nullable: false),
                    BudgetApproved = table.Column<decimal>(type: "REAL", nullable: false),
                    BudgetSource = table.Column<string>(type: "TEXT", nullable: false),
                    FundingSource = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovedBy = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApprovalNotes = table.Column<string>(type: "TEXT", nullable: false),
                    SelectedSupplierId = table.Column<int>(type: "INTEGER", nullable: true),
                    SelectionJustification = table.Column<string>(type: "TEXT", nullable: false),
                    PurchaseOrderNumber = table.Column<string>(type: "TEXT", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InstallationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedDeviceId = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementRequests_MedicalDevices_CreatedDeviceId",
                        column: x => x.CreatedDeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcurementRequests_Suppliers_SelectedSupplierId",
                        column: x => x.SelectedSupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RiskIncidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    IncidentCode = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReportedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IncidentLocation = table.Column<string>(type: "TEXT", nullable: false),
                    IncidentType = table.Column<string>(type: "TEXT", nullable: false),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false),
                    Probability = table.Column<int>(type: "INTEGER", nullable: false),
                    PatientInjury = table.Column<bool>(type: "INTEGER", nullable: false),
                    InjuryDescription = table.Column<string>(type: "TEXT", nullable: false),
                    StaffInjury = table.Column<bool>(type: "INTEGER", nullable: false),
                    StaffMembersInvolved = table.Column<string>(type: "TEXT", nullable: false),
                    PatientsInvolved = table.Column<string>(type: "TEXT", nullable: false),
                    AffectedPatients = table.Column<int>(type: "INTEGER", nullable: false),
                    AffectedStaff = table.Column<int>(type: "INTEGER", nullable: false),
                    RootCause = table.Column<string>(type: "TEXT", nullable: false),
                    InvestigationFindings = table.Column<string>(type: "TEXT", nullable: false),
                    InvestigationCompleteDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CorrectiveActions = table.Column<string>(type: "TEXT", nullable: false),
                    PreventiveActions = table.Column<string>(type: "TEXT", nullable: false),
                    ActionDeadline = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRecall = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecallNumber = table.Column<string>(type: "TEXT", nullable: false),
                    RecallAuthority = table.Column<string>(type: "TEXT", nullable: false),
                    RecallDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RegulatoryReported = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegulatoryReportDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RegulatoryReportReference = table.Column<string>(type: "TEXT", nullable: false),
                    Resolution = table.Column<string>(type: "TEXT", nullable: false),
                    ResolvedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResolvedBy = table.Column<string>(type: "TEXT", nullable: false),
                    Attachments = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskIncidents_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpareParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PartName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompatibleModels = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: true),
                    SupplierName = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentStock = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumStock = table.Column<int>(type: "INTEGER", nullable: false),
                    MaximumStock = table.Column<int>(type: "INTEGER", nullable: false),
                    ReorderPoint = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitCost = table.Column<decimal>(type: "REAL", nullable: false),
                    StorageLocation = table.Column<string>(type: "TEXT", nullable: false),
                    LastOrderDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastOrderCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCritical = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsObsolete = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastUsedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TotalUsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpareParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpareParts_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpareParts_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StaffMemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeviceCategory = table.Column<string>(type: "TEXT", nullable: false),
                    TrainingTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TrainingDescription = table.Column<string>(type: "TEXT", nullable: false),
                    TrainingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Trainer = table.Column<string>(type: "TEXT", nullable: false),
                    TrainingProvider = table.Column<string>(type: "TEXT", nullable: false),
                    CertificationNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentPassed = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssessmentScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    AttachmentPath = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingRecords_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingRecords_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalibrationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    CalibrationType = table.Column<string>(type: "TEXT", nullable: false),
                    StandardUsed = table.Column<string>(type: "TEXT", nullable: false),
                    StandardCertificate = table.Column<string>(type: "TEXT", nullable: false),
                    CalibrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextDueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PerformedBy = table.Column<string>(type: "TEXT", nullable: false),
                    PerformedByStaffId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsExternalLab = table.Column<bool>(type: "INTEGER", nullable: false),
                    LaboratoryName = table.Column<string>(type: "TEXT", nullable: false),
                    AccreditationNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<int>(type: "INTEGER", nullable: false),
                    AsFoundData = table.Column<string>(type: "TEXT", nullable: false),
                    AsLeftData = table.Column<string>(type: "TEXT", nullable: false),
                    ToleranceLimits = table.Column<string>(type: "TEXT", nullable: false),
                    MeasurementUncertainty = table.Column<string>(type: "TEXT", nullable: false),
                    Temperature = table.Column<decimal>(type: "TEXT", nullable: true),
                    Humidity = table.Column<decimal>(type: "TEXT", nullable: true),
                    EnvironmentalConditions = table.Column<string>(type: "TEXT", nullable: false),
                    CalibrationProcedure = table.Column<string>(type: "TEXT", nullable: false),
                    AdjustmentsMade = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdjustmentDescription = table.Column<string>(type: "TEXT", nullable: false),
                    CertificateNumber = table.Column<string>(type: "TEXT", nullable: false),
                    TechnicianSignature = table.Column<string>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: false),
                    Attachments = table.Column<string>(type: "TEXT", nullable: false),
                    ElectricalSafetyTestId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalibrationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalibrationRecords_ElectricalSafetyTests_ElectricalSafetyTestId",
                        column: x => x.ElectricalSafetyTestId,
                        principalTable: "ElectricalSafetyTests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CalibrationRecords_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalibrationRecords_StaffMembers_PerformedByStaffId",
                        column: x => x.PerformedByStaffId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkOrderNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportedBy = table.Column<string>(type: "TEXT", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FaultDescription = table.Column<string>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceCategory = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedTo = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsExternalContractor = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContractorName = table.Column<string>(type: "TEXT", nullable: false),
                    ScheduledStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ScheduledEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ResolutionDescription = table.Column<string>(type: "TEXT", nullable: false),
                    RootCause = table.Column<string>(type: "TEXT", nullable: false),
                    ResponseTimeHours = table.Column<int>(type: "INTEGER", nullable: true),
                    ResolutionTimeHours = table.Column<int>(type: "INTEGER", nullable: true),
                    SLADeadline = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    ActualCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaintenanceRecordId = table.Column<int>(type: "INTEGER", nullable: true),
                    Attachments = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_MaintenanceRecords_MaintenanceRecordId",
                        column: x => x.MaintenanceRecordId,
                        principalTable: "MaintenanceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WorkOrders_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceActionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NetworkDeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionType = table.Column<string>(type: "TEXT", nullable: false),
                    ActionDescription = table.Column<string>(type: "TEXT", nullable: false),
                    Parameters = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<int>(type: "INTEGER", nullable: false),
                    ResultMessage = table.Column<string>(type: "TEXT", nullable: false),
                    ExecutedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceActionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceActionLogs_NetworkDevices_NetworkDeviceId",
                        column: x => x.NetworkDeviceId,
                        principalTable: "NetworkDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcurementRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    TechnicalScore = table.Column<int>(type: "INTEGER", nullable: false),
                    FinancialScore = table.Column<int>(type: "INTEGER", nullable: false),
                    QualityScore = table.Column<int>(type: "INTEGER", nullable: false),
                    SupportScore = table.Column<int>(type: "INTEGER", nullable: false),
                    DeliveryScore = table.Column<int>(type: "INTEGER", nullable: false),
                    QuotedPrice = table.Column<decimal>(type: "REAL", nullable: false),
                    TotalCostOfOwnership = table.Column<decimal>(type: "REAL", nullable: false),
                    PaymentTerms = table.Column<string>(type: "TEXT", nullable: false),
                    WarrantyYears = table.Column<int>(type: "INTEGER", nullable: false),
                    TechnicalCompliance = table.Column<string>(type: "TEXT", nullable: false),
                    Deviations = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryTimeframe = table.Column<string>(type: "TEXT", nullable: false),
                    IsSelected = table.Column<bool>(type: "INTEGER", nullable: false),
                    EvaluationNotes = table.Column<string>(type: "TEXT", nullable: false),
                    EvaluatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicalEvaluations_ProcurementRequests_ProcurementRequestId",
                        column: x => x.ProcurementRequestId,
                        principalTable: "ProcurementRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnicalEvaluations_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SparePartId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaintenanceRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityUsed = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    UsedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartUsages_MaintenanceRecords_MaintenanceRecordId",
                        column: x => x.MaintenanceRecordId,
                        principalTable: "MaintenanceRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SparePartUsages_SpareParts_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "SpareParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedDevices_DeviceId",
                table: "AssignedDevices",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedDevices_StaffMemberId_DeviceId",
                table: "AssignedDevices",
                columns: new[] { "StaffMemberId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserName",
                table: "AuditLogs",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_CalibrationDate",
                table: "CalibrationRecords",
                column: "CalibrationDate");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_DeviceId",
                table: "CalibrationRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_ElectricalSafetyTestId",
                table: "CalibrationRecords",
                column: "ElectricalSafetyTestId");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_NextDueDate",
                table: "CalibrationRecords",
                column: "NextDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_PerformedByStaffId",
                table: "CalibrationRecords",
                column: "PerformedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceActionLogs_ActionType",
                table: "DeviceActionLogs",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceActionLogs_CreatedAt",
                table: "DeviceActionLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceActionLogs_NetworkDeviceId",
                table: "DeviceActionLogs",
                column: "NetworkDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceActionLogs_Result",
                table: "DeviceActionLogs",
                column: "Result");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDocuments_DeviceId_DocumentType",
                table: "DeviceDocuments",
                columns: new[] { "DeviceId", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_ElectricalSafetyTests_DeviceId",
                table: "ElectricalSafetyTests",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectricalSafetyTests_NextDueDate",
                table: "ElectricalSafetyTests",
                column: "NextDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ElectricalSafetyTests_TestDate",
                table: "ElectricalSafetyTests",
                column: "TestDate");

            migrationBuilder.CreateIndex(
                name: "IX_LicensedDevices_HardwareFingerprint",
                table: "LicensedDevices",
                column: "HardwareFingerprint");

            migrationBuilder.CreateIndex(
                name: "IX_LicensedDevices_LicenseInfoId",
                table: "LicensedDevices",
                column: "LicenseInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensedDevices_MacAddress",
                table: "LicensedDevices",
                column: "MacAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_LicenseKey",
                table: "Licenses",
                column: "LicenseKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_DeviceId",
                table: "MaintenanceRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_PerformedByStaffId",
                table: "MaintenanceRecords",
                column: "PerformedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_ScheduledDate",
                table: "MaintenanceRecords",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_AssignedStaffId",
                table: "MedicalDevices",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_DepartmentId",
                table: "MedicalDevices",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_DeviceCode",
                table: "MedicalDevices",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_SerialNumber",
                table: "MedicalDevices",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_SupplierId",
                table: "MedicalDevices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkDevices_ConnectionStatus",
                table: "NetworkDevices",
                column: "ConnectionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkDevices_DeviceName",
                table: "NetworkDevices",
                column: "DeviceName");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkDevices_Hostname",
                table: "NetworkDevices",
                column: "Hostname");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkDevices_IpAddress",
                table: "NetworkDevices",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkDevices_MacAddress",
                table: "NetworkDevices",
                column: "MacAddress");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkDevices_MedicalDeviceId",
                table: "NetworkDevices",
                column: "MedicalDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DueDate",
                table: "Notifications",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Priority",
                table: "Notifications",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_CreatedDeviceId",
                table: "ProcurementRequests",
                column: "CreatedDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_RequestNumber",
                table: "ProcurementRequests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_SelectedSupplierId",
                table: "ProcurementRequests",
                column: "SelectedSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_Status",
                table: "ProcurementRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIncidents_DeviceId",
                table: "RiskIncidents",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIncidents_IncidentCode",
                table: "RiskIncidents",
                column: "IncidentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskIncidents_IncidentDate",
                table: "RiskIncidents",
                column: "IncidentDate");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContracts_ContractNumber",
                table: "ServiceContracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContracts_SupplierId",
                table: "ServiceContracts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_DeviceId",
                table: "SpareParts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_PartNumber",
                table: "SpareParts",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_SupplierId",
                table: "SpareParts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartUsages_MaintenanceRecordId",
                table: "SparePartUsages",
                column: "MaintenanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartUsages_SparePartId",
                table: "SparePartUsages",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_DepartmentId",
                table: "StaffMembers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_EmployeeId",
                table: "StaffMembers",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_SupplierCode",
                table: "Suppliers",
                column: "SupplierCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalEvaluations_ProcurementRequestId_SupplierId",
                table: "TechnicalEvaluations",
                columns: new[] { "ProcurementRequestId", "SupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicalEvaluations_SupplierId",
                table: "TechnicalEvaluations",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRecords_DeviceId",
                table: "TrainingRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRecords_StaffMemberId_DeviceId_TrainingDate",
                table: "TrainingRecords",
                columns: new[] { "StaffMemberId", "DeviceId", "TrainingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_IsActive",
                table: "UserSessions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_DeviceId",
                table: "WorkOrders",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_MaintenanceRecordId",
                table: "WorkOrders",
                column: "MaintenanceRecordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ReportDate",
                table: "WorkOrders",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Status",
                table: "WorkOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkOrderNumber",
                table: "WorkOrders",
                column: "WorkOrderNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedDevices");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CalibrationRecords");

            migrationBuilder.DropTable(
                name: "DeviceActionLogs");

            migrationBuilder.DropTable(
                name: "DeviceDocuments");

            migrationBuilder.DropTable(
                name: "LicensedDevices");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RiskIncidents");

            migrationBuilder.DropTable(
                name: "ServiceContracts");

            migrationBuilder.DropTable(
                name: "SparePartUsages");

            migrationBuilder.DropTable(
                name: "TechnicalEvaluations");

            migrationBuilder.DropTable(
                name: "TrainingRecords");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "ElectricalSafetyTests");

            migrationBuilder.DropTable(
                name: "NetworkDevices");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DropTable(
                name: "ProcurementRequests");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MaintenanceRecords");

            migrationBuilder.DropTable(
                name: "MedicalDevices");

            migrationBuilder.DropTable(
                name: "StaffMembers");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomenMedmSys.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Supplier",
                table: "SpareParts",
                newName: "SupplierName");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                table: "MedicalDevices",
                newName: "WarrantyTerms");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "SpareParts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedDate",
                table: "SpareParts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReorderPoint",
                table: "SpareParts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "SpareParts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalUsageCount",
                table: "SpareParts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "ServiceContracts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContractFilePath",
                table: "ServiceContracts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PenaltyClause",
                table: "ServiceContracts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RenewalNoticeDate",
                table: "ServiceContracts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RenewalNoticeDays",
                table: "ServiceContracts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "ServiceContracts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarrantyCertificatePath",
                table: "ServiceContracts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "RiskIncidents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientsInvolved",
                table: "RiskIncidents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegulatoryReportDate",
                table: "RiskIncidents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegulatoryReportReference",
                table: "RiskIncidents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RegulatoryReported",
                table: "RiskIncidents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StaffMembersInvolved",
                table: "RiskIncidents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AcquisitionMethod",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssetTagNumber",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AssignedStaffId",
                table: "MedicalDevices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CommissioningDate",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentDepreciatedValue",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "MedicalDevices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepreciationMethod",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EstimatedLifespanYears",
                table: "MedicalDevices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDisposalDate",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InstallationDate",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCalibrationDate",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenanceDate",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSafetyTestDate",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceManualPath",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "NetworkConnected",
                table: "MedicalDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OperationalManualPath",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RFIDTag",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegulatoryClass",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RequiresElectricalSafetyTesting",
                table: "MedicalDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SoftwareVersion",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "MedicalDevices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalMaintenanceCost",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UdiCode",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarrantyCertificatePath",
                table: "MedicalDevices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DowntimeHours",
                table: "MaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedCompletionDate",
                table: "MaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformedByStaffId",
                table: "MaintenanceRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "MaintenanceRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RootCause",
                table: "MaintenanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationDate",
                table: "MaintenanceRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VerificationPerformed",
                table: "MaintenanceRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "MaintenanceRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "MaintenanceRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "CalibrationRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CalibrationProcedure",
                table: "CalibrationRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ElectricalSafetyTestId",
                table: "CalibrationRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnvironmentalConditions",
                table: "CalibrationRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Humidity",
                table: "CalibrationRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformedByStaffId",
                table: "CalibrationRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicianSignature",
                table: "CalibrationRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "CalibrationRecords",
                type: "TEXT",
                nullable: true);

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
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Specialization = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: false),
                    HireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Certifications = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseNumber = table.Column<string>(type: "TEXT", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_SupplierId",
                table: "SpareParts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceContracts_SupplierId",
                table: "ServiceContracts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_AssignedStaffId",
                table: "MedicalDevices",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_DepartmentId",
                table: "MedicalDevices",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalDevices_SupplierId",
                table: "MedicalDevices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_PerformedByStaffId",
                table: "MaintenanceRecords",
                column: "PerformedByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_ElectricalSafetyTestId",
                table: "CalibrationRecords",
                column: "ElectricalSafetyTestId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CalibrationRecords_ElectricalSafetyTests_ElectricalSafetyTestId",
                table: "CalibrationRecords",
                column: "ElectricalSafetyTestId",
                principalTable: "ElectricalSafetyTests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CalibrationRecords_StaffMembers_PerformedByStaffId",
                table: "CalibrationRecords",
                column: "PerformedByStaffId",
                principalTable: "StaffMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRecords_StaffMembers_PerformedByStaffId",
                table: "MaintenanceRecords",
                column: "PerformedByStaffId",
                principalTable: "StaffMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalDevices_Departments_DepartmentId",
                table: "MedicalDevices",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalDevices_StaffMembers_AssignedStaffId",
                table: "MedicalDevices",
                column: "AssignedStaffId",
                principalTable: "StaffMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalDevices_Suppliers_SupplierId",
                table: "MedicalDevices",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceContracts_Suppliers_SupplierId",
                table: "ServiceContracts",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpareParts_Suppliers_SupplierId",
                table: "SpareParts",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalibrationRecords_ElectricalSafetyTests_ElectricalSafetyTestId",
                table: "CalibrationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_CalibrationRecords_StaffMembers_PerformedByStaffId",
                table: "CalibrationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRecords_StaffMembers_PerformedByStaffId",
                table: "MaintenanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalDevices_Departments_DepartmentId",
                table: "MedicalDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalDevices_StaffMembers_AssignedStaffId",
                table: "MedicalDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalDevices_Suppliers_SupplierId",
                table: "MedicalDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceContracts_Suppliers_SupplierId",
                table: "ServiceContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_SpareParts_Suppliers_SupplierId",
                table: "SpareParts");

            migrationBuilder.DropTable(
                name: "DeviceDocuments");

            migrationBuilder.DropTable(
                name: "ElectricalSafetyTests");

            migrationBuilder.DropTable(
                name: "TechnicalEvaluations");

            migrationBuilder.DropTable(
                name: "TrainingRecords");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "ProcurementRequests");

            migrationBuilder.DropTable(
                name: "StaffMembers");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_SpareParts_SupplierId",
                table: "SpareParts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceContracts_SupplierId",
                table: "ServiceContracts");

            migrationBuilder.DropIndex(
                name: "IX_MedicalDevices_AssignedStaffId",
                table: "MedicalDevices");

            migrationBuilder.DropIndex(
                name: "IX_MedicalDevices_DepartmentId",
                table: "MedicalDevices");

            migrationBuilder.DropIndex(
                name: "IX_MedicalDevices_SupplierId",
                table: "MedicalDevices");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRecords_PerformedByStaffId",
                table: "MaintenanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_CalibrationRecords_ElectricalSafetyTestId",
                table: "CalibrationRecords");

            migrationBuilder.DropIndex(
                name: "IX_CalibrationRecords_PerformedByStaffId",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "LastUsedDate",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "ReorderPoint",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "TotalUsageCount",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "ServiceContracts");

            migrationBuilder.DropColumn(
                name: "ContractFilePath",
                table: "ServiceContracts");

            migrationBuilder.DropColumn(
                name: "PenaltyClause",
                table: "ServiceContracts");

            migrationBuilder.DropColumn(
                name: "RenewalNoticeDate",
                table: "ServiceContracts");

            migrationBuilder.DropColumn(
                name: "RenewalNoticeDays",
                table: "ServiceContracts");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "ServiceContracts");

            migrationBuilder.DropColumn(
                name: "WarrantyCertificatePath",
                table: "ServiceContracts");

            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "RiskIncidents");

            migrationBuilder.DropColumn(
                name: "PatientsInvolved",
                table: "RiskIncidents");

            migrationBuilder.DropColumn(
                name: "RegulatoryReportDate",
                table: "RiskIncidents");

            migrationBuilder.DropColumn(
                name: "RegulatoryReportReference",
                table: "RiskIncidents");

            migrationBuilder.DropColumn(
                name: "RegulatoryReported",
                table: "RiskIncidents");

            migrationBuilder.DropColumn(
                name: "StaffMembersInvolved",
                table: "RiskIncidents");

            migrationBuilder.DropColumn(
                name: "AcquisitionMethod",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "AssetTagNumber",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "AssignedStaffId",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "CommissioningDate",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "CurrentDepreciatedValue",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "DepreciationMethod",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "EstimatedLifespanYears",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "ExpectedDisposalDate",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "InstallationDate",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "LastCalibrationDate",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "LastMaintenanceDate",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "LastSafetyTestDate",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "MaintenanceManualPath",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "NetworkConnected",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "OperationalManualPath",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "RFIDTag",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "RegulatoryClass",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "RequiresElectricalSafetyTesting",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "SoftwareVersion",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "TotalMaintenanceCost",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "UdiCode",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "WarrantyCertificatePath",
                table: "MedicalDevices");

            migrationBuilder.DropColumn(
                name: "DowntimeHours",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "EstimatedCompletionDate",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "PerformedByStaffId",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "RootCause",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "VerificationDate",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "VerificationPerformed",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "CalibrationProcedure",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "ElectricalSafetyTestId",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "EnvironmentalConditions",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "PerformedByStaffId",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "TechnicianSignature",
                table: "CalibrationRecords");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "CalibrationRecords");

            migrationBuilder.RenameColumn(
                name: "SupplierName",
                table: "SpareParts",
                newName: "Supplier");

            migrationBuilder.RenameColumn(
                name: "WarrantyTerms",
                table: "MedicalDevices",
                newName: "Supplier");
        }
    }
}

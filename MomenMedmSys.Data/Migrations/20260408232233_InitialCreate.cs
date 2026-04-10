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
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "REAL", nullable: false),
                    Supplier = table.Column<string>(type: "TEXT", nullable: false),
                    WarrantyProvider = table.Column<string>(type: "TEXT", nullable: false),
                    WarrantyExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    Building = table.Column<string>(type: "TEXT", nullable: false),
                    Floor = table.Column<string>(type: "TEXT", nullable: false),
                    Room = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedTo = table.Column<string>(type: "TEXT", nullable: false),
                    TechnicalSpecs = table.Column<string>(type: "TEXT", nullable: false),
                    PowerRequirements = table.Column<string>(type: "TEXT", nullable: false),
                    RequiresCalibration = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresPreventiveMaintenance = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    DisposalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisposalReason = table.Column<string>(type: "TEXT", nullable: false),
                    RiskClassification = table.Column<int>(type: "INTEGER", nullable: false),
                    SafetyNotes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ContractName = table.Column<string>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPerson = table.Column<string>(type: "TEXT", nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    CoverageDescription = table.Column<string>(type: "TEXT", nullable: false),
                    ContractValue = table.Column<decimal>(type: "REAL", nullable: false),
                    PaymentTerms = table.Column<string>(type: "TEXT", nullable: false),
                    CoveredDeviceCategories = table.Column<string>(type: "TEXT", nullable: false),
                    CoveredDeviceIds = table.Column<string>(type: "TEXT", nullable: false),
                    ResponseTimeHours = table.Column<int>(type: "INTEGER", nullable: false),
                    ResolutionTimeHours = table.Column<int>(type: "INTEGER", nullable: false),
                    SLADetails = table.Column<string>(type: "TEXT", nullable: false),
                    TotalCalls = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedCalls = table.Column<int>(type: "INTEGER", nullable: false),
                    SatisfactionScore = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceContracts", x => x.Id);
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
                    IsExternalLab = table.Column<bool>(type: "INTEGER", nullable: false),
                    LaboratoryName = table.Column<string>(type: "TEXT", nullable: false),
                    AccreditationNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<int>(type: "INTEGER", nullable: false),
                    AsFoundData = table.Column<string>(type: "TEXT", nullable: false),
                    AsLeftData = table.Column<string>(type: "TEXT", nullable: false),
                    ToleranceLimits = table.Column<string>(type: "TEXT", nullable: false),
                    MeasurementUncertainty = table.Column<string>(type: "TEXT", nullable: false),
                    AdjustmentsMade = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdjustmentDescription = table.Column<string>(type: "TEXT", nullable: false),
                    CertificateNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalibrationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalibrationRecords_MedicalDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    IsExternalContractor = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContractorName = table.Column<string>(type: "TEXT", nullable: false),
                    ContractReference = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Findings = table.Column<string>(type: "TEXT", nullable: false),
                    ActionsTaken = table.Column<string>(type: "TEXT", nullable: false),
                    Recommendations = table.Column<string>(type: "TEXT", nullable: false),
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
                    Resolution = table.Column<string>(type: "TEXT", nullable: false),
                    ResolvedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResolvedBy = table.Column<string>(type: "TEXT", nullable: false),
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
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompatibleModels = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Supplier = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentStock = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumStock = table.Column<int>(type: "INTEGER", nullable: false),
                    MaximumStock = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitCost = table.Column<decimal>(type: "REAL", nullable: false),
                    StorageLocation = table.Column<string>(type: "TEXT", nullable: false),
                    LastOrderDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastOrderCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCritical = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsObsolete = table.Column<bool>(type: "INTEGER", nullable: false),
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
                name: "IX_CalibrationRecords_CalibrationDate",
                table: "CalibrationRecords",
                column: "CalibrationDate");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_DeviceId",
                table: "CalibrationRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationRecords_NextDueDate",
                table: "CalibrationRecords",
                column: "NextDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_DeviceId",
                table: "MaintenanceRecords",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRecords_ScheduledDate",
                table: "MaintenanceRecords",
                column: "ScheduledDate");

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
                name: "IX_SpareParts_DeviceId",
                table: "SpareParts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_PartNumber",
                table: "SpareParts",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SparePartUsages_MaintenanceRecordId",
                table: "SparePartUsages",
                column: "MaintenanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartUsages_SparePartId",
                table: "SparePartUsages",
                column: "SparePartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalibrationRecords");

            migrationBuilder.DropTable(
                name: "RiskIncidents");

            migrationBuilder.DropTable(
                name: "ServiceContracts");

            migrationBuilder.DropTable(
                name: "SparePartUsages");

            migrationBuilder.DropTable(
                name: "MaintenanceRecords");

            migrationBuilder.DropTable(
                name: "SpareParts");

            migrationBuilder.DropTable(
                name: "MedicalDevices");
        }
    }
}

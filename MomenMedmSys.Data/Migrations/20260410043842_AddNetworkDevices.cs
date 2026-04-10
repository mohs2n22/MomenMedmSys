using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomenMedmSys.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNetworkDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceActionLogs");

            migrationBuilder.DropTable(
                name: "NetworkDevices");
        }
    }
}

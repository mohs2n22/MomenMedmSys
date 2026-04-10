using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomenMedmSys.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminPanelAndStaffRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "CanAccessAdminPanel",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageCalibration",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageDevices",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageMaintenance",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageNetworkDevices",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageSpareParts",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanManageStaff",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewReports",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActiveAccount",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "StaffMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "StaffMembers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "StaffMembers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubRole",
                table: "StaffMembers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "StaffMembers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_AssignedDevices_DeviceId",
                table: "AssignedDevices",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedDevices_StaffMemberId_DeviceId",
                table: "AssignedDevices",
                columns: new[] { "StaffMemberId", "DeviceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedDevices");

            migrationBuilder.DropColumn(
                name: "CanAccessAdminPanel",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "CanManageCalibration",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "CanManageDevices",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "CanManageMaintenance",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "CanManageNetworkDevices",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "CanManageSpareParts",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "CanManageStaff",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "CanViewReports",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "IsActiveAccount",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "SubRole",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "StaffMembers");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "StaffMembers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}

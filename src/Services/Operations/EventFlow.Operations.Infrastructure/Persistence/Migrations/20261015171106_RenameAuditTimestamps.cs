using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace EventFlow.Operations.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class RenameAuditTimestamps : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "Notifications",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "AttendanceStaffAssignments",
            newName: "CreatedAt");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "AttendanceRecords",
            newName: "CreatedAt");

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "Notifications",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "AttendanceStaffAssignments",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "AttendanceRecords",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "Notifications");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "AttendanceStaffAssignments");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "AttendanceRecords");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "Notifications",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "AttendanceStaffAssignments",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "AttendanceRecords",
            newName: "CreatedAtUtc");
    }
}

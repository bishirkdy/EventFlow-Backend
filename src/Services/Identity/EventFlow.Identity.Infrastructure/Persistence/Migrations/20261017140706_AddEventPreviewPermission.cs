using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace EventFlow.Identity.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddEventPreviewPermission : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Permissions",
            columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
            values: new object[] { new Guid("20000000-0000-0000-0000-000000000012"), new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Preview event website", "event.preview", null });

        migrationBuilder.InsertData(
            table: "RolePermissions",
            columns: new[] { "PermissionId", "RoleId" },
            values: new object[] { new Guid("20000000-0000-0000-0000-000000000012"), new Guid("10000000-0000-0000-0000-000000000001") });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "RolePermissions",
            keyColumns: new[] { "PermissionId", "RoleId" },
            keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000012"), new Guid("10000000-0000-0000-0000-000000000001") });

        migrationBuilder.DeleteData(
            table: "Permissions",
            keyColumn: "Id",
            keyValue: new Guid("20000000-0000-0000-0000-000000000012"));
    }
}

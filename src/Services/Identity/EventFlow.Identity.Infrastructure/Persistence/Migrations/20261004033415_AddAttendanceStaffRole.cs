using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventFlow.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceStaffRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000007"), new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Event attendance staff", "AttendanceStaff", null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000007") },
                    { new Guid("20000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000007") },
                    { new Guid("20000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000007") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000007") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000006"), new Guid("10000000-0000-0000-0000-000000000007") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000007"), new Guid("10000000-0000-0000-0000-000000000007") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace EventFlow.Registration.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddRegistrationScope : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "ScopeId",
            table: "Tickets",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ScopeType",
            table: "Tickets",
            type: "integer",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.AddColumn<Guid>(
            name: "ScopeId",
            table: "Registrations",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ScopeType",
            table: "Registrations",
            type: "integer",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.CreateIndex(
            name: "IX_Registrations_EventId_ScopeType_ScopeId",
            table: "Registrations",
            columns: new[] { "EventId", "ScopeType", "ScopeId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Registrations_EventId_ScopeType_ScopeId",
            table: "Registrations");

        migrationBuilder.DropColumn(
            name: "ScopeId",
            table: "Tickets");

        migrationBuilder.DropColumn(
            name: "ScopeType",
            table: "Tickets");

        migrationBuilder.DropColumn(
            name: "ScopeId",
            table: "Registrations");

        migrationBuilder.DropColumn(
            name: "ScopeType",
            table: "Registrations");
    }
}

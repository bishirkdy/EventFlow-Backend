using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Registration.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationFormScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrationForms_EventId",
                table: "RegistrationForms");

            migrationBuilder.AddColumn<Guid>(
                name: "ScopeId",
                table: "RegistrationForms",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScopeType",
                table: "RegistrationForms",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationForms_EventId",
                table: "RegistrationForms",
                column: "EventId",
                unique: true,
                filter: "\"ScopeId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationForms_EventId_ScopeType_ScopeId",
                table: "RegistrationForms",
                columns: new[] { "EventId", "ScopeType", "ScopeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrationForms_EventId",
                table: "RegistrationForms");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationForms_EventId_ScopeType_ScopeId",
                table: "RegistrationForms");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "RegistrationForms");

            migrationBuilder.DropColumn(
                name: "ScopeType",
                table: "RegistrationForms");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationForms_EventId",
                table: "RegistrationForms",
                column: "EventId",
                unique: true);
        }
    }
}

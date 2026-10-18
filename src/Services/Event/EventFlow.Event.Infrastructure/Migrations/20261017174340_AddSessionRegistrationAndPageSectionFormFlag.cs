using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddSessionRegistrationAndPageSectionFormFlag : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "AllowRegistration",
            table: "Sessions",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "ShowRegistrationForm",
            table: "PageSections",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AllowRegistration",
            table: "Sessions");

        migrationBuilder.DropColumn(
            name: "ShowRegistrationForm",
            table: "PageSections");
    }
}

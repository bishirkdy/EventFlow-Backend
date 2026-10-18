using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Identity.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddPlatformAdminToUsers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsPlatformAdmin",
            table: "Users",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsPlatformAdmin",
            table: "Users");
    }
}

using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations;

[DbContext(typeof(EventCoreDbContext))]
[Migration("20260926080000_SimplifyNavigation")]
public partial class SimplifyNavigation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Location",
            table: "NavigationMenus");

        migrationBuilder.DropColumn(
            name: "OpenInNewTab",
            table: "NavigationItems");

        migrationBuilder.DropColumn(
            name: "Url",
            table: "NavigationItems");

        migrationBuilder.Sql(
            @"DELETE FROM ""NavigationItems"" WHERE ""PageId"" IS NULL;");

        migrationBuilder.AlterColumn<Guid>(
            name: "PageId",
            table: "NavigationItems",
            type: "uuid",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "PageId",
            table: "NavigationItems",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");

        migrationBuilder.AddColumn<string>(
            name: "Url",
            table: "NavigationItems",
            type: "character varying(1000)",
            maxLength: 1000,
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "OpenInNewTab",
            table: "NavigationItems",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Location",
            table: "NavigationMenus",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "Header");
    }
}

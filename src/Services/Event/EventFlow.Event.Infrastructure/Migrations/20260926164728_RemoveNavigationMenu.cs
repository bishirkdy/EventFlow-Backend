using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNavigationMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "NavigationItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
        UPDATE "NavigationItems" AS ni
        SET "EventId" = nm."EventId"
        FROM "NavigationMenus" AS nm
        WHERE ni."NavigationMenuId" = nm."Id";
        """);

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "NavigationItems",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_NavigationItems_NavigationMenus_NavigationMenuId",
                table: "NavigationItems");

            migrationBuilder.DropIndex(
                name: "IX_NavigationItems_NavigationMenuId_DisplayOrder",
                table: "NavigationItems");

            migrationBuilder.DropColumn(
                name: "NavigationMenuId",
                table: "NavigationItems");


            migrationBuilder.CreateIndex(
                name: "IX_NavigationItems_EventId_DisplayOrder",
                table: "NavigationItems",
                columns: new[] { "EventId", "DisplayOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems",
                column: "PageId",
                principalTable: "EventPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NavigationItems_Events_EventId",
                table: "NavigationItems",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropTable(
                name: "NavigationMenus");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems");

            migrationBuilder.DropForeignKey(
                name: "FK_NavigationItems_Events_EventId",
                table: "NavigationItems");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "NavigationItems",
                newName: "NavigationMenuId");

            migrationBuilder.RenameIndex(
                name: "IX_NavigationItems_EventId_DisplayOrder",
                table: "NavigationItems",
                newName: "IX_NavigationItems_NavigationMenuId_DisplayOrder");

            migrationBuilder.AlterColumn<Guid>(
                name: "PageId",
                table: "NavigationItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<bool>(
                name: "OpenInNewTab",
                table: "NavigationItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "NavigationItems",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NavigationMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavigationMenus_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationMenus_EventId",
                table: "NavigationMenus",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems",
                column: "PageId",
                principalTable: "EventPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NavigationItems_NavigationMenus_NavigationMenuId",
                table: "NavigationItems",
                column: "NavigationMenuId",
                principalTable: "NavigationMenus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

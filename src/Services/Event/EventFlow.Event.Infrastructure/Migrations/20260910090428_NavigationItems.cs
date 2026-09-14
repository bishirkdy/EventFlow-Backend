using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NavigationItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NavigationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NavigationMenuId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PageId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    OpenInNewTab = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavigationItems_EventPages_PageId",
                        column: x => x.PageId,
                        principalTable: "EventPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NavigationItems_NavigationMenus_NavigationMenuId",
                        column: x => x.NavigationMenuId,
                        principalTable: "NavigationMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationItems_NavigationMenuId_DisplayOrder",
                table: "NavigationItems",
                columns: new[] { "NavigationMenuId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationItems_PageId",
                table: "NavigationItems",
                column: "PageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavigationItems");
        }
    }
}

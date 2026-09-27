using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNavigationUnusedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems");



            migrationBuilder.AddForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems",
                column: "PageId",
                principalTable: "EventPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems");


            migrationBuilder.AddForeignKey(
                name: "FK_NavigationItems_EventPages_PageId",
                table: "NavigationItems",
                column: "PageId",
                principalTable: "EventPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventImageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                 name: "PublicId",
                 table: "EventImage");
            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "EventImage",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "EventImage",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "EventImage",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "EventImage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.DropIndex(
                name: "IX_EventImage_EventId_DisplayOrder",
                table: "EventImage");

            migrationBuilder.CreateIndex(
                name: "IX_EventImage_EventId",
                table: "EventImage",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventImage_EventId_DisplayOrder",
                table: "EventImage",
                columns: new[] { "EventId", "DisplayOrder" },
                unique: true);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventImage_EventId_DisplayOrder",
                table: "EventImage");

            migrationBuilder.DropIndex(
                name: "IX_EventImage_EventId",
                table: "EventImage");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "EventImage");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "EventImage");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "EventImage");

            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "EventImage");

            migrationBuilder.CreateIndex(
                name: "IX_EventImage_EventId_DisplayOrder",
                table: "EventImage",
                columns: new[] { "EventId", "DisplayOrder" });

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "EventImage",
                type: "character varying",
                nullable: false,
                defaultValue: "");
        }
    }
}

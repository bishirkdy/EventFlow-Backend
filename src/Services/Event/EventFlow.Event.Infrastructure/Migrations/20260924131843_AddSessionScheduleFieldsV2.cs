using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    public partial class AddSessionScheduleFieldsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartTimeUtc",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTimeUtc",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VenueId",
                table: "Sessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_VenueId",
                table: "Sessions",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Venues_VenueId",
                table: "Sessions",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Venues_VenueId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_VenueId",
                table: "Sessions");

            migrationBuilder.DropColumn(name: "StartTimeUtc", table: "Sessions");
            migrationBuilder.DropColumn(name: "EndTimeUtc", table: "Sessions");
            migrationBuilder.DropColumn(name: "VenueId", table: "Sessions");
        }
    }
}

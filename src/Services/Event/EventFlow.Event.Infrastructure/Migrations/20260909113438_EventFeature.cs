using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EventFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFeatures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventFeatures_EventId_FeatureId",
                table: "EventFeatures",
                columns: new[] { "EventId", "FeatureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventFeatures");
        }
    }
}

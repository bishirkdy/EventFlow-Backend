using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypeFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEnabledByDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypeFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTypeFeatures_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTypeFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "schedule", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage the event schedule.", true, "Schedule", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "sessions", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage event sessions.", true, "Sessions", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "venues", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage event venues.", true, "Venues", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "speakers", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage event speakers.", true, "Speakers", null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "sponsors", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage event sponsors.", true, "Sponsors", null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "registration", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage participant registration.", true, "Registration", null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "attendance", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage participant attendance.", true, "Attendance", null },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "gallery", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage event photos and gallery.", true, "Gallery", null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), "certificates", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manage event certificates.", true, "Certificates", null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "feedback", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Collect participant feedback.", true, "Feedback", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventFeatures_FeatureId",
                table: "EventFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTypeFeatures_EventTypeId_FeatureId",
                table: "EventTypeFeatures",
                columns: new[] { "EventTypeId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventTypeFeatures_FeatureId",
                table: "EventTypeFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Code",
                table: "Features",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventFeatures_Features_FeatureId",
                table: "EventFeatures",
                column: "FeatureId",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventFeatures_Features_FeatureId",
                table: "EventFeatures");

            migrationBuilder.DropTable(
                name: "EventTypeFeatures");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropIndex(
                name: "IX_EventFeatures_FeatureId",
                table: "EventFeatures");
        }
    }
}

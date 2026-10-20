using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackFeatureToAllEventTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventTypeFeatures",
                columns: new[] { "Id", "CreatedAt", "EventTypeId", "FeatureId", "IsEnabledByDefault", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000036"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("10000000-0000-0000-0000-000000000010"), false, null },
                    { new Guid("20000000-0000-0000-0000-000000000037"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000010"), false, null },
                    { new Guid("20000000-0000-0000-0000-000000000038"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000010"), false, null }
                });

            migrationBuilder.Sql("""
                INSERT INTO "EventFeatures" ("Id", "CreatedAt", "EventId", "FeatureId", "IsEnabled", "UpdatedAt")
                SELECT gen_random_uuid(), now(), e."Id", '10000000-0000-0000-0000-000000000010', false, NULL
                FROM "Events" e
                WHERE NOT EXISTS (
                    SELECT 1 FROM "EventFeatures" ef
                    WHERE ef."EventId" = e."Id"
                      AND ef."FeatureId" = '10000000-0000-0000-0000-000000000010');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000036"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000037"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000038"));

            migrationBuilder.Sql("""
                DELETE FROM "EventFeatures"
                WHERE "FeatureId" = '10000000-0000-0000-0000-000000000010'
                  AND "IsEnabled" = false;
                """);
        }
    }
}

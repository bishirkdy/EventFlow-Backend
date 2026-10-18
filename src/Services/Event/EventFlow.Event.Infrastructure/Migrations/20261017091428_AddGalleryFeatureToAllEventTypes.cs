using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventFlow.Event.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddGalleryFeatureToAllEventTypes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "EventTypeFeatures",
            columns: new[] { "Id", "CreatedAt", "EventTypeId", "FeatureId", "IsEnabledByDefault", "UpdatedAt" },
            values: new object[,]
            {
                { new Guid("20000000-0000-0000-0000-000000000034"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000008"), false, null },
                { new Guid("20000000-0000-0000-0000-000000000035"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000008"), false, null }
            });

        migrationBuilder.Sql("""
            INSERT INTO "EventFeatures" ("Id", "CreatedAt", "EventId", "FeatureId", "IsEnabled", "UpdatedAt")
            SELECT gen_random_uuid(), now(), e."Id", '10000000-0000-0000-0000-000000000008', false, NULL
            FROM "Events" e
            WHERE NOT EXISTS (
                SELECT 1 FROM "EventFeatures" ef
                WHERE ef."EventId" = e."Id"
                  AND ef."FeatureId" = '10000000-0000-0000-0000-000000000008');
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "EventTypeFeatures",
            keyColumn: "Id",
            keyValue: new Guid("20000000-0000-0000-0000-000000000034"));

        migrationBuilder.DeleteData(
            table: "EventTypeFeatures",
            keyColumn: "Id",
            keyValue: new Guid("20000000-0000-0000-0000-000000000035"));

        migrationBuilder.Sql("""
            DELETE FROM "EventFeatures"
            WHERE "FeatureId" = '10000000-0000-0000-0000-000000000008'
              AND "IsEnabled" = false;
            """);
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTypeFeatureDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventTypeFeatures",
                columns: new[] { "Id", "CreatedAt", "EventTypeId", "FeatureId", "IsEnabledByDefault", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("10000000-0000-0000-0000-000000000001"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("10000000-0000-0000-0000-000000000003"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("10000000-0000-0000-0000-000000000008"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("10000000-0000-0000-0000-000000000006"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000001"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000002"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000003"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000004"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000005"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000006"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000007"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000012"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), new Guid("10000000-0000-0000-0000-000000000010"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000013"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000001"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000014"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000002"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000015"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000003"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000016"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000006"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000017"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000007"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000018"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000009"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000019"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("10000000-0000-0000-0000-000000000010"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000020"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000001"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000021"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000002"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000022"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000003"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000023"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000005"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000024"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000006"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000025"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000007"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000026"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444444"), new Guid("10000000-0000-0000-0000-000000000008"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000027"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000001"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000028"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000002"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000029"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000003"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000030"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000006"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000031"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000007"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000032"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000008"), true, null },
                    { new Guid("20000000-0000-0000-0000-000000000033"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("55555555-5555-5555-5555-555555555555"), new Guid("10000000-0000-0000-0000-000000000009"), true, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "EventTypeFeatures",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000033"));
        }
    }
}

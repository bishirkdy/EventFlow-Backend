using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create EventTypes table
            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false),

                    Code = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false),

                    Name = table.Column<string>(
                        type: "character varying(150)",
                        maxLength: 150,
                        nullable: false),

                    Description = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: true),

                    IsActive = table.Column<bool>(
                        type: "boolean",
                        nullable: false),

                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false),

                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            // 2. Seed EventTypes
            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[]
                {
                    "Id",
                    "Code",
                    "CreatedAt",
                    "Description",
                    "IsActive",
                    "Name",
                    "UpdatedAt"
                },
                values: new object[,]
                {
                    {
                        new Guid("11111111-1111-1111-1111-111111111111"),
                        "Wedding",
                        new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc),
                        "Weddings and private events",
                        true,
                        "Wedding & Private Events",
                        null
                    },
                    {
                        new Guid("22222222-2222-2222-2222-222222222222"),
                        "Conference",
                        new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc),
                        "Conferences, business events and professional gatherings",
                        true,
                        "Conference & Business",
                        null
                    },
                    {
                        new Guid("33333333-3333-3333-3333-333333333333"),
                        "Education",
                        new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc),
                        "Educational events, workshops and training programs",
                        true,
                        "Education & Workshop",
                        null
                    },
                    {
                        new Guid("44444444-4444-4444-4444-444444444444"),
                        "Festival",
                        new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc),
                        "Festivals and cultural events",
                        true,
                        "Festival & Cultural",
                        null
                    },
                    {
                        new Guid("55555555-5555-5555-5555-555555555555"),
                        "Sports",
                        new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc),
                        "Sports events and competitions",
                        true,
                        "Sports & Competition",
                        null
                    }
                });

            // 3. Add EventTypeId to existing Events
            // Existing events will temporarily use Wedding.
            migrationBuilder.AddColumn<Guid>(
                name: "EventTypeId",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid(
                    "11111111-1111-1111-1111-111111111111"));

            // 4. Create index
            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTypeId",
                table: "Events",
                column: "EventTypeId");

            // 5. Create foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventTypes_EventTypeId",
                table: "Events",
                column: "EventTypeId",
                principalTable: "EventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // 6. Remove old EventType column
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Events");

            // 7. Unique index for EventType Code
            migrationBuilder.CreateIndex(
                name: "IX_EventTypes_Code",
                table: "EventTypes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventTypes_EventTypeId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventTypeId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Events",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropIndex(
                name: "IX_EventTypes_Code",
                table: "EventTypes");

            migrationBuilder.DropTable(
                name: "EventTypes");
        }
    }
}
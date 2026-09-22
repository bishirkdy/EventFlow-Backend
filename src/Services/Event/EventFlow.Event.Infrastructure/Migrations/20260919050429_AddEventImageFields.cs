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
            migrationBuilder.CreateTable(
                name: "EventImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false),

                    EventId = table.Column<Guid>(
                        type: "uuid",
                        nullable: false),

                    Url = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: false),

                    StorageKey = table.Column<string>(
                        type: "character varying(1000)",
                        maxLength: 1000,
                        nullable: false),

                    OriginalFileName = table.Column<string>(
                        type: "character varying(255)",
                        maxLength: 255,
                        nullable: false),

                    ContentType = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false),

                    SizeBytes = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    DisplayOrder = table.Column<int>(
                        type: "integer",
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
                    table.PrimaryKey("PK_EventImage", x => x.Id);

                    table.ForeignKey(
                        name: "FK_EventImage_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventImage_EventId",
                table: "EventImage",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventImage_EventId_DisplayOrder",
                table: "EventImage",
                columns: new[]
                {
                    "EventId",
                    "DisplayOrder"
                },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventImage");
        }
    }
}
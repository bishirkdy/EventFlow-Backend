using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations;

[DbContext(typeof(EventCoreDbContext))]
[Migration("20260925100000_AddSpeakersSponsors")]
public partial class AddSpeakersSponsors : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Speakers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                EventId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Bio = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                Designation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Organization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Speakers", x => x.Id);
                table.ForeignKey("FK_Speakers_Events_EventId", x => x.EventId, "Events", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Sponsors",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                EventId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                WebsiteUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                LogoUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                SponsorLevel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sponsors", x => x.Id);
                table.ForeignKey("FK_Sponsors_Events_EventId", x => x.EventId, "Events", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SessionSpeakers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                SpeakerId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SessionSpeakers", x => x.Id);
                table.ForeignKey("FK_SessionSpeakers_Sessions_SessionId", x => x.SessionId, "Sessions", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_SessionSpeakers_Speakers_SpeakerId", x => x.SpeakerId, "Speakers", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Speakers_EventId", table: "Speakers", column: "EventId");
        migrationBuilder.CreateIndex(name: "IX_Speakers_EventId_DisplayOrder", table: "Speakers", columns: new[] { "EventId", "DisplayOrder" });
        migrationBuilder.CreateIndex(name: "IX_Sponsors_EventId", table: "Sponsors", column: "EventId");
        migrationBuilder.CreateIndex(name: "IX_Sponsors_EventId_SponsorLevel_DisplayOrder", table: "Sponsors", columns: new[] { "EventId", "SponsorLevel", "DisplayOrder" });
        migrationBuilder.CreateIndex(name: "IX_SessionSpeakers_SessionId_SpeakerId", table: "SessionSpeakers", columns: new[] { "SessionId", "SpeakerId" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_SessionSpeakers_SpeakerId", table: "SessionSpeakers", column: "SpeakerId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "SessionSpeakers");
        migrationBuilder.DropTable(name: "Sponsors");
        migrationBuilder.DropTable(name: "Speakers");
    }
}

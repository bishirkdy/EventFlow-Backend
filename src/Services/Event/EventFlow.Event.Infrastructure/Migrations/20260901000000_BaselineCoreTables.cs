using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    /// <summary>
    ///     Backdated baseline: the original migration that created the "Events" table was
    ///     lost from the repository, so every fresh database failed at the first migration
    ///     with a foreign key to "Events" (NavigationMenu). This migration runs before all
    ///     others and is idempotent: on fresh databases it creates "Events"; on databases
    ///     that already have it (existing dev data) it is a no-op.
    ///     Columns match the oldest surviving model snapshot
    ///     (20260908144416_AddEventSettings.Designer.cs).
    /// </summary>
    [DbContext(typeof(EventCoreDbContext))]
    [Migration("20260901000000_BaselineCoreTables")]
    public partial class BaselineCoreTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""Events"" (
    ""Id"" uuid NOT NULL,
    ""CreatedAt"" timestamp with time zone NOT NULL,
    ""CreatedBy"" uuid NOT NULL,
    ""Description"" character varying(2000) NULL,
    ""EndDate"" timestamp with time zone NOT NULL,
    ""EventType"" character varying(100) NOT NULL,
    ""Name"" character varying(200) NOT NULL,
    ""StartDate"" timestamp with time zone NOT NULL,
    ""Status"" integer NOT NULL,
    ""SubType"" character varying(100) NULL,
    ""Subdomain"" text NULL,
    ""TimeZone"" character varying(100) NOT NULL,
    ""UpdatedAt"" timestamp with time zone NULL,
    CONSTRAINT ""PK_Events"" PRIMARY KEY (""Id"")
);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""Events"";");
        }
    }
}

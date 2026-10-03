using EventFlow.Event.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Event.Infrastructure.Migrations
{
    [DbContext(typeof(EventCoreDbContext))]
    [Migration("20260924150000_AddSessionVenueImages")]
    public partial class AddSessionVenueImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""ALTER TABLE "Sessions" ADD COLUMN IF NOT EXISTS "ImageUrl" character varying(1000);""");
            migrationBuilder.Sql("""ALTER TABLE "Venues" ADD COLUMN IF NOT EXISTS "ImageUrl" character varying(1000);""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""ALTER TABLE "Sessions" DROP COLUMN IF EXISTS "ImageUrl";""");
            migrationBuilder.Sql("""ALTER TABLE "Venues" DROP COLUMN IF EXISTS "ImageUrl";""");
        }
    }
}

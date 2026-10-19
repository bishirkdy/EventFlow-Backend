using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Operations.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSectionAttendanceScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Section-scoped attendance staff no longer exist; only event and
            // session scopes are supported.
            migrationBuilder.Sql("DELETE FROM \"AttendanceStaffAssignments\" WHERE \"ScopeType\" = 2;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

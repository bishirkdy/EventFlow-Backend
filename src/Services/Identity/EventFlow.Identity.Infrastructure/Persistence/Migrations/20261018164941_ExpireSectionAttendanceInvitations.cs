using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpireSectionAttendanceInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Section scope is gone; revoke any still-pending section invitations
            // so they can no longer be accepted.
            migrationBuilder.Sql(
                "UPDATE \"EventInvitations\" SET \"Status\" = 2 " +
                "WHERE \"ScopeType\" = 2 AND \"Status\" = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

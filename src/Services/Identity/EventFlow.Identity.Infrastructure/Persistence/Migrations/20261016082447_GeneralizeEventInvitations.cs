using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace EventFlow.Identity.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class GeneralizeEventInvitations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_PhotographerInvitations_EventId_Email",
            table: "PhotographerInvitations");

        migrationBuilder.RenameTable(
            name: "PhotographerInvitations",
            newName: "EventInvitations");

        // Rename the primary key and foreign key constraints so the database
        // matches the renamed aggregate (Postgres keeps constraint names on
        // ALTER TABLE ... RENAME).
        migrationBuilder.Sql(
            @"ALTER TABLE ""EventInvitations"" RENAME CONSTRAINT ""PK_PhotographerInvitations"" TO ""PK_EventInvitations"";");

        migrationBuilder.Sql(
            @"ALTER TABLE ""EventInvitations"" RENAME CONSTRAINT ""FK_PhotographerInvitations_Roles_RoleId"" TO ""FK_EventInvitations_Roles_RoleId"";");

        migrationBuilder.Sql(
            @"ALTER TABLE ""EventInvitations"" RENAME CONSTRAINT ""FK_PhotographerInvitations_Users_CreatedBy"" TO ""FK_EventInvitations_Users_CreatedBy"";");

        migrationBuilder.Sql(
            @"ALTER TABLE ""EventInvitations"" RENAME CONSTRAINT ""FK_PhotographerInvitations_Users_AcceptedByUserId"" TO ""FK_EventInvitations_Users_AcceptedByUserId"";");

        migrationBuilder.RenameIndex(
            name: "IX_PhotographerInvitations_AcceptedByUserId",
            table: "EventInvitations",
            newName: "IX_EventInvitations_AcceptedByUserId");

        migrationBuilder.RenameIndex(
            name: "IX_PhotographerInvitations_CreatedBy",
            table: "EventInvitations",
            newName: "IX_EventInvitations_CreatedBy");

        migrationBuilder.RenameIndex(
            name: "IX_PhotographerInvitations_EventId",
            table: "EventInvitations",
            newName: "IX_EventInvitations_EventId");

        migrationBuilder.RenameIndex(
            name: "IX_PhotographerInvitations_RoleId",
            table: "EventInvitations",
            newName: "IX_EventInvitations_RoleId");

        migrationBuilder.RenameIndex(
            name: "IX_PhotographerInvitations_Status",
            table: "EventInvitations",
            newName: "IX_EventInvitations_Status");

        migrationBuilder.RenameIndex(
            name: "IX_PhotographerInvitations_Token",
            table: "EventInvitations",
            newName: "IX_EventInvitations_Token");

        // Existing invitations were all photographer invitations.
        migrationBuilder.AddColumn<int>(
            name: "Type",
            table: "EventInvitations",
            type: "integer",
            nullable: false,
            defaultValue: 1);

        migrationBuilder.AddColumn<int>(
            name: "ScopeType",
            table: "EventInvitations",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "ScopeId",
            table: "EventInvitations",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_EventInvitations_EventId_Type_Email",
            table: "EventInvitations",
            columns: new[] { "EventId", "Type", "Email" },
            unique: true,
            filter: "\"Status\" = 0");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_EventInvitations_EventId_Type_Email",
            table: "EventInvitations");

        migrationBuilder.DropColumn(
            name: "ScopeId",
            table: "EventInvitations");

        migrationBuilder.DropColumn(
            name: "ScopeType",
            table: "EventInvitations");

        migrationBuilder.DropColumn(
            name: "Type",
            table: "EventInvitations");

        migrationBuilder.RenameIndex(
            name: "IX_EventInvitations_AcceptedByUserId",
            table: "EventInvitations",
            newName: "IX_PhotographerInvitations_AcceptedByUserId");

        migrationBuilder.RenameIndex(
            name: "IX_EventInvitations_CreatedBy",
            table: "EventInvitations",
            newName: "IX_PhotographerInvitations_CreatedBy");

        migrationBuilder.RenameIndex(
            name: "IX_EventInvitations_EventId",
            table: "EventInvitations",
            newName: "IX_PhotographerInvitations_EventId");

        migrationBuilder.RenameIndex(
            name: "IX_EventInvitations_RoleId",
            table: "EventInvitations",
            newName: "IX_PhotographerInvitations_RoleId");

        migrationBuilder.RenameIndex(
            name: "IX_EventInvitations_Status",
            table: "EventInvitations",
            newName: "IX_PhotographerInvitations_Status");

        migrationBuilder.RenameIndex(
            name: "IX_EventInvitations_Token",
            table: "EventInvitations",
            newName: "IX_PhotographerInvitations_Token");

        migrationBuilder.RenameTable(
            name: "EventInvitations",
            newName: "PhotographerInvitations");

        migrationBuilder.Sql(
            @"ALTER TABLE ""PhotographerInvitations"" RENAME CONSTRAINT ""PK_EventInvitations"" TO ""PK_PhotographerInvitations"";");

        migrationBuilder.Sql(
            @"ALTER TABLE ""PhotographerInvitations"" RENAME CONSTRAINT ""FK_EventInvitations_Roles_RoleId"" TO ""FK_PhotographerInvitations_Roles_RoleId"";");

        migrationBuilder.Sql(
            @"ALTER TABLE ""PhotographerInvitations"" RENAME CONSTRAINT ""FK_EventInvitations_Users_CreatedBy"" TO ""FK_PhotographerInvitations_Users_CreatedBy"";");

        migrationBuilder.Sql(
            @"ALTER TABLE ""PhotographerInvitations"" RENAME CONSTRAINT ""FK_EventInvitations_Users_AcceptedByUserId"" TO ""FK_PhotographerInvitations_Users_AcceptedByUserId"";");

        migrationBuilder.CreateIndex(
            name: "IX_PhotographerInvitations_EventId_Email",
            table: "PhotographerInvitations",
            columns: new[] { "EventId", "Email" },
            unique: true,
            filter: "\"Status\" = 0");
    }
}

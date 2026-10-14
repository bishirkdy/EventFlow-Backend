using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Registration.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BackgroundImageHeight",
                table: "CertificateSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImagePublicId",
                table: "CertificateSettings",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageUrl",
                table: "CertificateSettings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BackgroundImageWidth",
                table: "CertificateSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TemplateFields",
                table: "CertificateSettings",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "TemplateMode",
                table: "CertificateSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImageHeight",
                table: "CertificateSettings");

            migrationBuilder.DropColumn(
                name: "BackgroundImagePublicId",
                table: "CertificateSettings");

            migrationBuilder.DropColumn(
                name: "BackgroundImageUrl",
                table: "CertificateSettings");

            migrationBuilder.DropColumn(
                name: "BackgroundImageWidth",
                table: "CertificateSettings");

            migrationBuilder.DropColumn(
                name: "TemplateFields",
                table: "CertificateSettings");

            migrationBuilder.DropColumn(
                name: "TemplateMode",
                table: "CertificateSettings");
        }
    }
}

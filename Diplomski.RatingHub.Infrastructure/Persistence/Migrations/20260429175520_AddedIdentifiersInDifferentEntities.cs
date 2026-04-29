using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.RatingHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedIdentifiersInDifferentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Companies",
                newName: "Verifier");

            migrationBuilder.AddColumn<string>(
                name: "AnonymousEditIdentifier",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "ReportedContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "CompanyVerificationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AnonymousEditIdentifier",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClaimCompanyIdentifier",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerifier",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnonymousEditIdentifier",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "ReportedContents");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "CompanyVerificationRequests");

            migrationBuilder.DropColumn(
                name: "AnonymousEditIdentifier",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ClaimCompanyIdentifier",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsEmailVerifier",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "Verifier",
                table: "Companies",
                newName: "Number");
        }
    }
}

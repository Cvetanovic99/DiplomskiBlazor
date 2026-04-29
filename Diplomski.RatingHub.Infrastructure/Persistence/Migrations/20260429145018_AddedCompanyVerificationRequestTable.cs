using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.RatingHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCompanyVerificationRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_UserProfiles_CreatorId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_CreatorId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Companies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CompanyVerificationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyVerificationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyVerificationRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyVerificationRequests_UserProfiles_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVerificationRequests_CompanyId",
                table: "CompanyVerificationRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyVerificationRequests_OwnerId",
                table: "CompanyVerificationRequests",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyVerificationRequests");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CreatorId",
                table: "Companies",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_UserProfiles_CreatorId",
                table: "Companies",
                column: "CreatorId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

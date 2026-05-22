using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.RatingHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangedForeignKeyInReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId",
                table: "CompanyResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyResponses_Reviews_ReviewId1",
                table: "CompanyResponses");

            migrationBuilder.DropIndex(
                name: "IX_CompanyResponses_ReviewId1",
                table: "CompanyResponses");

            migrationBuilder.RenameColumn(
                name: "ReviewId1",
                table: "CompanyResponses",
                newName: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyResponses_CompanyId1",
                table: "CompanyResponses",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId",
                table: "CompanyResponses",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId1",
                table: "CompanyResponses",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId",
                table: "CompanyResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId1",
                table: "CompanyResponses");

            migrationBuilder.DropIndex(
                name: "IX_CompanyResponses_CompanyId1",
                table: "CompanyResponses");

            migrationBuilder.RenameColumn(
                name: "CompanyId1",
                table: "CompanyResponses",
                newName: "ReviewId1");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyResponses_ReviewId1",
                table: "CompanyResponses",
                column: "ReviewId1",
                unique: true,
                filter: "[ReviewId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId",
                table: "CompanyResponses",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyResponses_Reviews_ReviewId1",
                table: "CompanyResponses",
                column: "ReviewId1",
                principalTable: "Reviews",
                principalColumn: "Id");
        }
    }
}

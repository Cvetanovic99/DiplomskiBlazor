using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.RatingHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCompanyResponseIdInReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_CompanyResponses_CompanyResponseId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CompanyResponseId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CompanyResponseId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CompanyResponseId1",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "ReviewId1",
                table: "CompanyResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyResponses_ReviewId1",
                table: "CompanyResponses",
                column: "ReviewId1",
                unique: true,
                filter: "[ReviewId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyResponses_Reviews_ReviewId1",
                table: "CompanyResponses",
                column: "ReviewId1",
                principalTable: "Reviews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyResponses_Reviews_ReviewId1",
                table: "CompanyResponses");

            migrationBuilder.DropIndex(
                name: "IX_CompanyResponses_ReviewId1",
                table: "CompanyResponses");

            migrationBuilder.DropColumn(
                name: "ReviewId1",
                table: "CompanyResponses");

            migrationBuilder.AddColumn<int>(
                name: "CompanyResponseId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyResponseId1",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CompanyResponseId1",
                table: "Reviews",
                column: "CompanyResponseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_CompanyResponses_CompanyResponseId1",
                table: "Reviews",
                column: "CompanyResponseId1",
                principalTable: "CompanyResponses",
                principalColumn: "Id");
        }
    }
}

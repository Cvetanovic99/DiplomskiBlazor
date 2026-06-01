using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.RatingHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixedEfFluentValidationForSomeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId1",
                table: "CompanyResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewGrades_RatingCriteria_RatingCriterionId1",
                table: "ReviewGrades");

            migrationBuilder.DropIndex(
                name: "IX_ReviewGrades_RatingCriterionId1",
                table: "ReviewGrades");

            migrationBuilder.DropIndex(
                name: "IX_CompanyResponses_CompanyId1",
                table: "CompanyResponses");

            migrationBuilder.DropColumn(
                name: "RatingCriterionId1",
                table: "ReviewGrades");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "CompanyResponses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatingCriterionId1",
                table: "ReviewGrades",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "CompanyResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewGrades_RatingCriterionId1",
                table: "ReviewGrades",
                column: "RatingCriterionId1");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyResponses_CompanyId1",
                table: "CompanyResponses",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyResponses_Companies_CompanyId1",
                table: "CompanyResponses",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewGrades_RatingCriteria_RatingCriterionId1",
                table: "ReviewGrades",
                column: "RatingCriterionId1",
                principalTable: "RatingCriteria",
                principalColumn: "Id");
        }
    }
}

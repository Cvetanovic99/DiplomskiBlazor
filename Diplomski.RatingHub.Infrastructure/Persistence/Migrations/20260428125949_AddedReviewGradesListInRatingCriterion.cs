using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.RatingHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedReviewGradesListInRatingCriterion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatingCriterionId1",
                table: "ReviewGrades",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewGrades_RatingCriterionId1",
                table: "ReviewGrades",
                column: "RatingCriterionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewGrades_RatingCriteria_RatingCriterionId1",
                table: "ReviewGrades",
                column: "RatingCriterionId1",
                principalTable: "RatingCriteria",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewGrades_RatingCriteria_RatingCriterionId1",
                table: "ReviewGrades");

            migrationBuilder.DropIndex(
                name: "IX_ReviewGrades_RatingCriterionId1",
                table: "ReviewGrades");

            migrationBuilder.DropColumn(
                name: "RatingCriterionId1",
                table: "ReviewGrades");
        }
    }
}

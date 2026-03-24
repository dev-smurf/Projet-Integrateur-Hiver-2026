using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScaleLabelsToQuizQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScaleMaxLabel",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Toujours");

            migrationBuilder.AddColumn<string>(
                name: "ScaleMidLabel",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Parfois");

            migrationBuilder.AddColumn<string>(
                name: "ScaleMinLabel",
                table: "QuizQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Jamais");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScaleMaxLabel",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "ScaleMidLabel",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "ScaleMinLabel",
                table: "QuizQuestions");
        }
    }
}

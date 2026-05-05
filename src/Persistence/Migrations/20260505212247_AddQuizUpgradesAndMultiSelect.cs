using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizUpgradesAndMultiSelect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuizAssignmentId",
                table: "UserQuizResponses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedResponseIds",
                table: "UserQuizResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableAt",
                table: "QuizAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpLabel",
                table: "QuizAssignments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "QuizAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizResponses_QuizAssignmentId",
                table: "UserQuizResponses",
                column: "QuizAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizResponses_QuizAssignments_QuizAssignmentId",
                table: "UserQuizResponses",
                column: "QuizAssignmentId",
                principalTable: "QuizAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizResponses_QuizAssignments_QuizAssignmentId",
                table: "UserQuizResponses");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizResponses_QuizAssignmentId",
                table: "UserQuizResponses");

            migrationBuilder.DropColumn(
                name: "QuizAssignmentId",
                table: "UserQuizResponses");

            migrationBuilder.DropColumn(
                name: "SelectedResponseIds",
                table: "UserQuizResponses");

            migrationBuilder.DropColumn(
                name: "AvailableAt",
                table: "QuizAssignments");

            migrationBuilder.DropColumn(
                name: "FollowUpLabel",
                table: "QuizAssignments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "QuizAssignments");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ajout_sousequipes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberEquipes_MemberId",
                table: "MemberEquipes");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentEquipeId",
                table: "Equipes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberEquipes_MemberId_EquipeId",
                table: "MemberEquipes",
                columns: new[] { "MemberId", "EquipeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipes_ParentEquipeId",
                table: "Equipes",
                column: "ParentEquipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipes_Equipes_ParentEquipeId",
                table: "Equipes",
                column: "ParentEquipeId",
                principalTable: "Equipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipes_Equipes_ParentEquipeId",
                table: "Equipes");

            migrationBuilder.DropIndex(
                name: "IX_MemberEquipes_MemberId_EquipeId",
                table: "MemberEquipes");

            migrationBuilder.DropIndex(
                name: "IX_Equipes_ParentEquipeId",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "ParentEquipeId",
                table: "Equipes");

            migrationBuilder.CreateIndex(
                name: "IX_MemberEquipes_MemberId",
                table: "MemberEquipes",
                column: "MemberId");
        }
    }
}
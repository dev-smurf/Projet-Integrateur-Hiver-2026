using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSousEquipesParentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Administrators_CreatedByAdminId",
                table: "MemberNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Members_MemberId",
                table: "MemberNotes");

            // ✅ Ajoute la colonne seulement si elle n'existe pas déjà
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Equipes' AND COLUMN_NAME = 'ParentEquipeId'
                )
                BEGIN
                    ALTER TABLE [Equipes] ADD [ParentEquipeId] uniqueidentifier NULL;
                END
            ");

            // ✅ Crée l'index seulement s'il n'existe pas déjà
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes 
                    WHERE name = 'IX_Equipes_ParentEquipeId' AND object_id = OBJECT_ID('Equipes')
                )
                BEGIN
                    CREATE INDEX [IX_Equipes_ParentEquipeId] ON [Equipes] ([ParentEquipeId]);
                END
            ");

            // ✅ Crée la FK seulement si elle n'existe pas déjà
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_Equipes_Equipes_ParentEquipeId'
                )
                BEGIN
                    ALTER TABLE [Equipes] ADD CONSTRAINT [FK_Equipes_Equipes_ParentEquipeId]
                    FOREIGN KEY ([ParentEquipeId]) REFERENCES [Equipes] ([Id])
                    ON DELETE NO ACTION;
                END
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberNotes_Administrators_CreatedByAdminId",
                table: "MemberNotes",
                column: "CreatedByAdminId",
                principalTable: "Administrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberNotes_Members_MemberId",
                table: "MemberNotes",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipes_Equipes_ParentEquipeId",
                table: "Equipes");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Administrators_CreatedByAdminId",
                table: "MemberNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Members_MemberId",
                table: "MemberNotes");

            migrationBuilder.DropIndex(
                name: "IX_Equipes_ParentEquipeId",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "ParentEquipeId",
                table: "Equipes");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberNotes_Administrators_CreatedByAdminId",
                table: "MemberNotes",
                column: "CreatedByAdminId",
                principalTable: "Administrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberNotes_Members_MemberId",
                table: "MemberNotes",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

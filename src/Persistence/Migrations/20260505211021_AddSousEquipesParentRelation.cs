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

<<<<<<< HEAD
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
=======
            migrationBuilder.Sql("""
                IF COL_LENGTH('Equipe', 'ParentEquipeId') IS NULL
                    ALTER TABLE [Equipe] ADD [ParentEquipeId] uniqueidentifier NULL;
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = 'IX_Equipes_ParentEquipeId'
                        AND [object_id] = OBJECT_ID('Equipe')
                )
                    CREATE INDEX [IX_Equipes_ParentEquipeId] ON [Equipe] ([ParentEquipeId]);
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE [name] = 'FK_Equipes_Equipes_ParentEquipeId'
                )
                    ALTER TABLE [Equipe] ADD CONSTRAINT [FK_Equipes_Equipes_ParentEquipeId]
                    FOREIGN KEY ([ParentEquipeId]) REFERENCES [Equipe] ([Id]) ON DELETE NO ACTION;
                """);
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3

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
<<<<<<< HEAD
            migrationBuilder.DropForeignKey(
                name: "FK_Equipes_Equipes_ParentEquipeId",
                table: "Equipes");
=======
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE [name] = 'FK_Equipes_Equipes_ParentEquipeId'
                )
                    ALTER TABLE [Equipe] DROP CONSTRAINT [FK_Equipes_Equipes_ParentEquipeId];
                """);
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3

            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Administrators_CreatedByAdminId",
                table: "MemberNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Members_MemberId",
                table: "MemberNotes");

<<<<<<< HEAD
            migrationBuilder.DropIndex(
                name: "IX_Equipes_ParentEquipeId",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "ParentEquipeId",
                table: "Equipes");
=======
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = 'IX_Equipes_ParentEquipeId'
                        AND [object_id] = OBJECT_ID('Equipe')
                )
                    DROP INDEX [IX_Equipes_ParentEquipeId] ON [Equipe];
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Equipe', 'ParentEquipeId') IS NOT NULL
                    ALTER TABLE [Equipe] DROP COLUMN [ParentEquipeId];
                """);
>>>>>>> 4895c6c70ef8607163c66e71f7a1785d1a95efe3

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

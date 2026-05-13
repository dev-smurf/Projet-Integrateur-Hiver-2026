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
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE [name] = 'FK_Equipes_Equipes_ParentEquipeId'
                )
                    ALTER TABLE [Equipe] DROP CONSTRAINT [FK_Equipes_Equipes_ParentEquipeId];
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Administrators_CreatedByAdminId",
                table: "MemberNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberNotes_Members_MemberId",
                table: "MemberNotes");

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

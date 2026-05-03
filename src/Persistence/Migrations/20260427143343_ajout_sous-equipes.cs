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
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_MemberEquipes_MemberId'
      AND object_id = OBJECT_ID(N'[dbo].[MemberEquipes]')
)
    DROP INDEX [IX_MemberEquipes_MemberId] ON [dbo].[MemberEquipes];
");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentEquipeId",
                table: "Equipes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[MemberEquipes]', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_MemberEquipes_MemberId_EquipeId'
      AND object_id = OBJECT_ID(N'[dbo].[MemberEquipes]')
)
    CREATE UNIQUE INDEX [IX_MemberEquipes_MemberId_EquipeId] ON [dbo].[MemberEquipes]([MemberId], [EquipeId]);
");

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

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[MemberEquipes]', N'U') IS NOT NULL
AND EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_MemberEquipes_MemberId_EquipeId'
      AND object_id = OBJECT_ID(N'[dbo].[MemberEquipes]')
)
    DROP INDEX [IX_MemberEquipes_MemberId_EquipeId] ON [dbo].[MemberEquipes];
");

            migrationBuilder.DropIndex(
                name: "IX_Equipes_ParentEquipeId",
                table: "Equipes");

            migrationBuilder.DropColumn(
                name: "ParentEquipeId",
                table: "Equipes");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_MemberEquipes_MemberId'
      AND object_id = OBJECT_ID(N'[dbo].[MemberEquipes]')
)
    CREATE INDEX [IX_MemberEquipes_MemberId] ON [dbo].[MemberEquipes]([MemberId]);
");
        }
    }
}

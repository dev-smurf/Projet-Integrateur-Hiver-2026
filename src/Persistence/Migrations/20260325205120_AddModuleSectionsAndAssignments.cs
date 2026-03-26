using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleSectionsAndAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename bilingual columns to single-language
            migrationBuilder.DropColumn(name: "ContenueEn", table: "Modules");
            migrationBuilder.DropColumn(name: "NameEn", table: "Modules");
            migrationBuilder.DropColumn(name: "SujetEn", table: "Modules");

            migrationBuilder.RenameColumn(name: "SujetFr", table: "Modules", newName: "Subject");
            migrationBuilder.RenameColumn(name: "NameFr", table: "Modules", newName: "Name");
            migrationBuilder.RenameColumn(name: "ContenueFr", table: "Modules", newName: "Content");

            // Remove max length constraints on Content
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            // Create ModuleSections table
            migrationBuilder.CreateTable(
                name: "ModuleSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleSections_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleSections_ModuleId_SortOrder",
                table: "ModuleSections",
                columns: new[] { "ModuleId", "SortOrder" });

            // Create MemberModules table
            migrationBuilder.CreateTable(
                name: "MemberModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgressPercent = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberModules_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberModules_MemberId_ModuleId",
                table: "MemberModules",
                columns: new[] { "MemberId", "ModuleId" },
                unique: true,
                filter: "Deleted IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MemberModules_ModuleId",
                table: "MemberModules",
                column: "ModuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MemberModules");
            migrationBuilder.DropTable(name: "ModuleSections");

            migrationBuilder.RenameColumn(name: "Subject", table: "Modules", newName: "SujetFr");
            migrationBuilder.RenameColumn(name: "Name", table: "Modules", newName: "NameFr");
            migrationBuilder.RenameColumn(name: "Content", table: "Modules", newName: "ContenueFr");

            migrationBuilder.AlterColumn<string>(
                name: "ContenueFr",
                table: "Modules",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(name: "ContenueEn", table: "Modules", type: "nvarchar(1000)", maxLength: 1000, nullable: true);
            migrationBuilder.AddColumn<string>(name: "NameEn", table: "Modules", type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "SujetEn", table: "Modules", type: "nvarchar(200)", maxLength: 200, nullable: true);
        }
    }
}

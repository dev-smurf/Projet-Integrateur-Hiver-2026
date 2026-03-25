using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModuleSectionsAndAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContenueEn",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "SujetEn",
                table: "Modules");

            migrationBuilder.RenameColumn(
                name: "SujetFr",
                table: "Modules",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "NameFr",
                table: "Modules",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ContenueFr",
                table: "Modules",
                newName: "Content");

            migrationBuilder.CreateTable(
                name: "MemberModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_MemberModules_MemberId_ModuleId",
                table: "MemberModules",
                columns: new[] { "MemberId", "ModuleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberModules_ModuleId",
                table: "MemberModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleSections_ModuleId_SortOrder",
                table: "ModuleSections",
                columns: new[] { "ModuleId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberModules");

            migrationBuilder.DropTable(
                name: "ModuleSections");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Modules",
                newName: "SujetFr");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Modules",
                newName: "NameFr");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Modules",
                newName: "ContenueFr");

            migrationBuilder.AddColumn<string>(
                name: "ContenueEn",
                table: "Modules",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Modules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SujetEn",
                table: "Modules",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}

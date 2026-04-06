using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberModuleSectionProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberModuleSectionProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberModuleSectionProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberModuleSectionProgress_MemberModules_MemberModuleId",
                        column: x => x.MemberModuleId,
                        principalTable: "MemberModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberModuleSectionProgress_ModuleSections_ModuleSectionId",
                        column: x => x.ModuleSectionId,
                        principalTable: "ModuleSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberModuleSectionProgress_MemberModuleId_ModuleSectionId",
                table: "MemberModuleSectionProgress",
                columns: new[] { "MemberModuleId", "ModuleSectionId" },
                unique: true,
                filter: "Deleted IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MemberModuleSectionProgress_ModuleSectionId",
                table: "MemberModuleSectionProgress",
                column: "ModuleSectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberModuleSectionProgress");
        }
    }
}

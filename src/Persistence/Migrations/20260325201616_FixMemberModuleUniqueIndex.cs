using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixMemberModuleUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberModules_MemberId_ModuleId",
                table: "MemberModules");

            migrationBuilder.CreateIndex(
                name: "IX_MemberModules_MemberId_ModuleId",
                table: "MemberModules",
                columns: new[] { "MemberId", "ModuleId" },
                unique: true,
                filter: "Deleted IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberModules_MemberId_ModuleId",
                table: "MemberModules");

            migrationBuilder.CreateIndex(
                name: "IX_MemberModules_MemberId_ModuleId",
                table: "MemberModules",
                columns: new[] { "MemberId", "ModuleId" },
                unique: true);
        }
    }
}

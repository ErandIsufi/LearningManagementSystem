using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eStudentSystem.Migrations
{
    /// <inheritdoc />
    public partial class MigirmiHunda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NgjyraFlokve",
                table: "Students",
                newName: "NgjyraHunes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NgjyraHunes",
                table: "Students",
                newName: "NgjyraFlokve");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class corrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "priority",
                table: "Task",
                newName: "Priority");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Task",
                newName: "priority");
        }
    }
}

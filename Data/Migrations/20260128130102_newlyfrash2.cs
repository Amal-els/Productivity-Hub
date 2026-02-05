using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class newlyfrash2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "priority",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "priority",
                table: "Task");
        }
    }
}

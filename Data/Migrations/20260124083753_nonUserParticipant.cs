using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class nonUserParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventParticipants_CalendarEventId_UserId",
                table: "EventParticipants");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EventParticipants",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EventParticipants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_CalendarEventId_UserId",
                table: "EventParticipants",
                columns: new[] { "CalendarEventId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventParticipants_CalendarEventId_UserId",
                table: "EventParticipants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "EventParticipants");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EventParticipants",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_CalendarEventId_UserId",
                table: "EventParticipants",
                columns: new[] { "CalendarEventId", "UserId" },
                unique: true);
        }
    }
}

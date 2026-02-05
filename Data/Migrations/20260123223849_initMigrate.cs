using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class initMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "todolist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_todolist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_todolist_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateOfCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateOfCompletion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Acheived = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    toDoListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_todolist_toDoListId",
                        column: x => x.toDoListId,
                        principalTable: "todolist",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Task_toDoListId",
                table: "Task",
                column: "toDoListId");

            migrationBuilder.CreateIndex(
                name: "IX_todolist_UserId",
                table: "todolist",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "todolist");
        }
    }
}

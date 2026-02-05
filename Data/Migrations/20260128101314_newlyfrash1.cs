using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class newlyfrash1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_todolist_AspNetUsers_UserId",
                table: "todolist");

            migrationBuilder.DropIndex(
                name: "IX_todolist_UserId",
                table: "todolist");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "todolist",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "toDoListId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_toDoListId",
                table: "AspNetUsers",
                column: "toDoListId",
                unique: true,
                filter: "[toDoListId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_todolist_toDoListId",
                table: "AspNetUsers",
                column: "toDoListId",
                principalTable: "todolist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_todolist_toDoListId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_toDoListId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "toDoListId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "todolist",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_todolist_UserId",
                table: "todolist",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_todolist_AspNetUsers_UserId",
                table: "todolist",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

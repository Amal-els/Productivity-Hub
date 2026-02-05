using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class @fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_todolist_toDoListId",
                table: "Task");

            migrationBuilder.RenameColumn(
                name: "toDoListId",
                table: "Task",
                newName: "ToDoListId");

            migrationBuilder.RenameIndex(
                name: "IX_Task_toDoListId",
                table: "Task",
                newName: "IX_Task_ToDoListId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ToDoListId",
                table: "Task",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Acheived",
                table: "Task",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_todolist_ToDoListId",
                table: "Task",
                column: "ToDoListId",
                principalTable: "todolist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_todolist_ToDoListId",
                table: "Task");

            migrationBuilder.RenameColumn(
                name: "ToDoListId",
                table: "Task",
                newName: "toDoListId");

            migrationBuilder.RenameIndex(
                name: "IX_Task_ToDoListId",
                table: "Task",
                newName: "IX_Task_toDoListId");

            migrationBuilder.AlterColumn<Guid>(
                name: "toDoListId",
                table: "Task",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Acheived",
                table: "Task",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_todolist_toDoListId",
                table: "Task",
                column: "toDoListId",
                principalTable: "todolist",
                principalColumn: "Id");
        }
    }
}

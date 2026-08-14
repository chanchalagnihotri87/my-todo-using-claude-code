using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TodoTask_Status_Added_Todo_Status_Removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TodoTasks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.Sql("UPDATE TodoTasks SET Status = 'Completed' WHERE IsCompleted = 1;");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TodoTasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "TodoTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Todos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "TodoTasks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TodoTasks");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TodoTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Todos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Todos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");
        }
    }
}

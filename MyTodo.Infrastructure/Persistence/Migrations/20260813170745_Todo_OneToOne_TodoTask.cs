using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Todo_OneToOne_TodoTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Todos_TodoTaskId",
                table: "Todos");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_TodoTaskId",
                table: "Todos",
                column: "TodoTaskId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Todos_TodoTaskId",
                table: "Todos");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_TodoTaskId",
                table: "Todos",
                column: "TodoTaskId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Objective_Status_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Objectives",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NotStarted");

            migrationBuilder.Sql(
                "UPDATE [Objectives] SET [Status] = 'Completed' WHERE [IsCompleted] = 1");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Objectives");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Objectives");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Objectives",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Problem_Status_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Problems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Problems");
        }
    }
}

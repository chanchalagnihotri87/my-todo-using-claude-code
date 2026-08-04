using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Problem_LifeAreaId_ForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LifeAreaId",
                table: "Problems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Problems_LifeAreaId",
                table: "Problems",
                column: "LifeAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_LifeAreas_LifeAreaId",
                table: "Problems",
                column: "LifeAreaId",
                principalTable: "LifeAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_LifeAreas_LifeAreaId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_LifeAreaId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "LifeAreaId",
                table: "Problems");
        }
    }
}

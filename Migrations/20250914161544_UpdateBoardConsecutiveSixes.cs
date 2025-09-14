using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LudoAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBoardConsecutiveSixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastConsecutiveSixes",
                table: "Boards",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastConsecutiveSixes",
                table: "Boards");
        }
    }
}

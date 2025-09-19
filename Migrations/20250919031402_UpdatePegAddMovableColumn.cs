using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LudoAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePegAddMovableColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Movable",
                table: "Pegs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Movable",
                table: "Pegs");
        }
    }
}

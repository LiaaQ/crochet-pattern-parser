using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrochetPatternParser.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToPatterns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Patterns",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Patterns");
        }
    }
}

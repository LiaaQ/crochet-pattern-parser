using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrochetPatternParser.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToPatterns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Patterns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Patterns");
        }
    }
}

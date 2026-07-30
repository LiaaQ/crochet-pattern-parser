using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrochetPatternParser.Migrations
{
    public partial class AddSectionName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SectionName",
                table: "Sections",
                type: "TEXT",
                nullable: false,
                defaultValue: "Section");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionName",
                table: "Sections");
        }
    }
}
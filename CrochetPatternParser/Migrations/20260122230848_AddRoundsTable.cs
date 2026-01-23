using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrochetPatternParser.Migrations
{
    /// <inheritdoc />
    public partial class AddRoundsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawText",
                table: "Patterns");

            migrationBuilder.CreateTable(
                name: "RoundEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoundNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    PatternId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoundEntity_Patterns_PatternId",
                        column: x => x.PatternId,
                        principalTable: "Patterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoundEntity_PatternId",
                table: "RoundEntity",
                column: "PatternId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoundEntity");

            migrationBuilder.AddColumn<string>(
                name: "RawText",
                table: "Patterns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

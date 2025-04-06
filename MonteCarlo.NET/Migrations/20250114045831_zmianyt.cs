using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class zmianyt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "czyPrzyznanoNagrode",
                table: "Zaklady",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "czyWygral",
                table: "Zaklady",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "czyPrzyznanoNagrode",
                table: "Zaklady");

            migrationBuilder.DropColumn(
                name: "czyWygral",
                table: "Zaklady");
        }
    }
}

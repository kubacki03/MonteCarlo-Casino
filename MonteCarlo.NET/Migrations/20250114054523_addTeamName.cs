using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class addTeamName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwayTeamName",
                table: "Mecz",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HomeTeamName",
                table: "Mecz",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayTeamName",
                table: "Mecz");

            migrationBuilder.DropColumn(
                name: "HomeTeamName",
                table: "Mecz");
        }
    }
}

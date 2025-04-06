using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class dodalemMecze : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Druzyna",
                columns: table => new
                {
                    DruzynaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    League = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Druzyna", x => x.DruzynaId);
                });

            migrationBuilder.CreateTable(
                name: "Mecz",
                columns: table => new
                {
                    MeczId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    HomeTeamId = table.Column<int>(type: "int", nullable: false),
                    AwayTeamId = table.Column<int>(type: "int", nullable: false),
                    HomeTeamGoals = table.Column<int>(type: "int", nullable: false),
                    AwayTeamGoals = table.Column<int>(type: "int", nullable: false),
                    HomeTeamOdds = table.Column<float>(type: "real", nullable: false),
                    AwayTeamOdds = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mecz", x => x.MeczId);
                    table.ForeignKey(
                        name: "FK_Mecz_Druzyna_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Druzyna",
                        principalColumn: "DruzynaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mecz_Druzyna_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Druzyna",
                        principalColumn: "DruzynaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mecz_AwayTeamId",
                table: "Mecz",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Mecz_HomeTeamId",
                table: "Mecz",
                column: "HomeTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mecz");

            migrationBuilder.DropTable(
                name: "Druzyna");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Gra",
                columns: new[] { "IdGry", "MinStawka", "Nazwa" },
                values: new object[,]
                {
                    { 1, 1.0, "Slotsy" },
                    { 2, 1.0, "Zdrapka koniczynka" },
                    { 3, 1.0, "Zdrapka Prosta" },
                    { 4, 1.0, "Wyscigi Konne" },
                    { 5, 1.0, "Obstawianie" },
                    { 6, 1.0, "Ruletka" },
                    { 7, 1.0, "Kosci" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Gra",
                keyColumn: "IdGry",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Gra",
                keyColumn: "IdGry",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Gra",
                keyColumn: "IdGry",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Gra",
                keyColumn: "IdGry",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Gra",
                keyColumn: "IdGry",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Gra",
                keyColumn: "IdGry",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Gra",
                keyColumn: "IdGry",
                keyValue: 7);
        }
    }
}

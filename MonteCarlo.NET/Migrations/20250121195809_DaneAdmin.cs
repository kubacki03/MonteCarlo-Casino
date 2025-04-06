using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class DaneAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c8167db9-cd8a-4f95-b827-a50cee08f971", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f16f0c7a-4a36-47ec-9a1b-969896e48979", 0, "eabc5dd9-84a8-4009-9da4-6b7b5ed16262", "admin@gmail.com", true, "Admin", null, false, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEEsP6UXzxP9uapoJJKsqIyqyWbd1rz9oOSpqokTz8JysOufsNQYPCuZ5LDeUEH2c5w==", null, false, null, "317de0d0-7ee8-4b50-a252-62888f863b00", false, "admin" });

            migrationBuilder.InsertData(
                table: "Levele",
                columns: new[] { "Id", "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[,]
                {
                    { 1, 5L, 1 },
                    { 2, 15L, 2 },
                    { 3, 50L, 3 },
                    { 4, 100L, 4 },
                    { 5, 500L, 5 },
                    { 6, 1000L, 6 },
                    { 7, 0L, 0 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c8167db9-cd8a-4f95-b827-a50cee08f971", "f16f0c7a-4a36-47ec-9a1b-969896e48979" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c8167db9-cd8a-4f95-b827-a50cee08f971", "f16f0c7a-4a36-47ec-9a1b-969896e48979" });

            migrationBuilder.DeleteData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8167db9-cd8a-4f95-b827-a50cee08f971");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f16f0c7a-4a36-47ec-9a1b-969896e48979");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class DodanieDanych : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "94d17dcb-9e64-4f3a-9d59-1c3e7a061187", "f2a541ba-4362-4293-ac46-544462370ddc" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94d17dcb-9e64-4f3a-9d59-1c3e7a061187");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f2a541ba-4362-4293-ac46-544462370ddc");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c483665d-4392-43a8-a590-7c0dc482d29a", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ba70459a-1412-4bc6-8676-9b80f3e64817", 0, "4f13a96b-3941-44eb-b534-e63e68ba5546", "admin@gmail.com", false, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEBVGAq3lA4GqZ3uinsn7p/okqpBRtmSwpHxl41hHgtPaqhBIBsZR3mnDimZUZx/pmQ==", null, false, 0.0, "69f3e650-3681-41a6-a303-4857fd13f41c", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "Druzyna",
                columns: new[] { "DruzynaId", "League", "Name" },
                values: new object[,]
                {
                    { 1, "Premier League", "Olimpia .NeT" },
                    { 2, "Premier League", "Java FC" }
                });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 0L, 0 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 5L, 1 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 15L, 2 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 50L, 3 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 100L, 4 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 500L, 5 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 1000L, 6 });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c483665d-4392-43a8-a590-7c0dc482d29a", "ba70459a-1412-4bc6-8676-9b80f3e64817" });

            migrationBuilder.InsertData(
                table: "Mecz",
                columns: new[] { "MeczId", "AwayTeamGoals", "AwayTeamId", "AwayTeamName", "AwayTeamOdds", "HomeTeamGoals", "HomeTeamId", "HomeTeamName", "HomeTeamOdds", "data" },
                values: new object[,]
                {
                    { 1, 2, 1, "Java FC", 2f, 2, 2, "Olimpia .NeT", 3f, new DateOnly(2025, 2, 2) },
                    { 2, 2, 1, "Java FC", 2f, 2, 2, "Olimpia .NeT", 3f, new DateOnly(2025, 2, 3) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c483665d-4392-43a8-a590-7c0dc482d29a", "ba70459a-1412-4bc6-8676-9b80f3e64817" });

            migrationBuilder.DeleteData(
                table: "Mecz",
                keyColumn: "MeczId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Mecz",
                keyColumn: "MeczId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c483665d-4392-43a8-a590-7c0dc482d29a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ba70459a-1412-4bc6-8676-9b80f3e64817");

            migrationBuilder.DeleteData(
                table: "Druzyna",
                keyColumn: "DruzynaId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Druzyna",
                keyColumn: "DruzynaId",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "94d17dcb-9e64-4f3a-9d59-1c3e7a061187", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f2a541ba-4362-4293-ac46-544462370ddc", 0, "666fbea0-065c-4119-9297-ab372056630c", "admin@gmail.com", false, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAECQ8PpDT0cKaZOfZZEQJEJ2aw7raBqolgyv90xOMfP/KmpNOX3l40snFbFx9o1FxdA==", null, false, 0.0, "40fda3a3-866f-4663-b938-5f07fc9a62b4", false, "admin@gmail.com" });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 5L, 1 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 15L, 2 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 50L, 3 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 100L, 4 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 500L, 5 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 1000L, 6 });

            migrationBuilder.UpdateData(
                table: "Levele",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "MinimumPlayedGames", "NumberOfLevel" },
                values: new object[] { 0L, 0 });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "94d17dcb-9e64-4f3a-9d59-1c3e7a061187", "f2a541ba-4362-4293-ac46-544462370ddc" });
        }
    }
}

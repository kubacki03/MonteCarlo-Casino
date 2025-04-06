using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class PoparawaAdminBaza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2c544969-92e7-47c0-8a66-b298b012b3af", "225a7582-0494-449c-86b3-9f50cbee6196" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c544969-92e7-47c0-8a66-b298b012b3af");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "225a7582-0494-449c-86b3-9f50cbee6196");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "39b76d8c-64f4-4377-bca0-13c1587e9b75", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b20ce6c6-64cb-40c4-8d6b-68b27fa08ab4", 0, "813cb214-d5f5-403a-89ed-9332693918d9", "admin@gmail.com", true, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEE2FIGHp9QSWJ1u+OvVyMXF9BmYj3oeQka/i1iR0gEtRh4C3gdXBStG6K8dkflX5Aw==", null, false, 0.0, "a8b3a1d6-e0ca-43b9-8975-873e072dd180", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "39b76d8c-64f4-4377-bca0-13c1587e9b75", "b20ce6c6-64cb-40c4-8d6b-68b27fa08ab4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "39b76d8c-64f4-4377-bca0-13c1587e9b75", "b20ce6c6-64cb-40c4-8d6b-68b27fa08ab4" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39b76d8c-64f4-4377-bca0-13c1587e9b75");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b20ce6c6-64cb-40c4-8d6b-68b27fa08ab4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2c544969-92e7-47c0-8a66-b298b012b3af", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "225a7582-0494-449c-86b3-9f50cbee6196", 0, "2a1ddda5-43eb-4238-88b5-02cc2b2edca5", "admin@gmail.com", true, "Admin", 0, false, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAELEAI00UDI6bc3Aa/Ul8ceqOwQ108u8jSEFtBUl5b2m3BoNN/xKDbyVJV8yj28NSng==", null, false, 0.0, "210d1cb4-68ec-4642-a660-d421c4607198", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "2c544969-92e7-47c0-8a66-b298b012b3af", "225a7582-0494-449c-86b3-9f50cbee6196" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class PoprawaAdmina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c8167db9-cd8a-4f95-b827-a50cee08f971", "f16f0c7a-4a36-47ec-9a1b-969896e48979" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8167db9-cd8a-4f95-b827-a50cee08f971");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f16f0c7a-4a36-47ec-9a1b-969896e48979");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { "c8167db9-cd8a-4f95-b827-a50cee08f971", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f16f0c7a-4a36-47ec-9a1b-969896e48979", 0, "eabc5dd9-84a8-4009-9da4-6b7b5ed16262", "admin@gmail.com", true, "Admin", null, false, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEEsP6UXzxP9uapoJJKsqIyqyWbd1rz9oOSpqokTz8JysOufsNQYPCuZ5LDeUEH2c5w==", null, false, null, "317de0d0-7ee8-4b50-a252-62888f863b00", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c8167db9-cd8a-4f95-b827-a50cee08f971", "f16f0c7a-4a36-47ec-9a1b-969896e48979" });
        }
    }
}

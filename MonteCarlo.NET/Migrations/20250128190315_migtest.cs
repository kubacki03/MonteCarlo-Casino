using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class migtest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0ab43ecd-2a41-4900-82b2-62cb0e6a6414", "9f5325be-d52d-4519-aa0e-395ea11fe138" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ab43ecd-2a41-4900-82b2-62cb0e6a6414");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9f5325be-d52d-4519-aa0e-395ea11fe138");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1d05fa48-1c86-41b9-850a-0d69268207c6", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "de46f4b9-2872-40e7-a3cd-ca0874c767eb", 0, "b59c25ec-5bb6-40dc-8899-0dc4da6b3542", "admin@gmail.com", false, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEOMq5PA3vVfus87WD/DxYk4dWZj3JkkXU4OFimrPQs4Na4z5+LgbpCxMo5S+kZvJ/g==", null, false, 0.0, "060d2513-157d-4cda-aab4-4988857d6ac3", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1d05fa48-1c86-41b9-850a-0d69268207c6", "de46f4b9-2872-40e7-a3cd-ca0874c767eb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1d05fa48-1c86-41b9-850a-0d69268207c6", "de46f4b9-2872-40e7-a3cd-ca0874c767eb" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1d05fa48-1c86-41b9-850a-0d69268207c6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "de46f4b9-2872-40e7-a3cd-ca0874c767eb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0ab43ecd-2a41-4900-82b2-62cb0e6a6414", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "9f5325be-d52d-4519-aa0e-395ea11fe138", 0, "e828c135-434e-4f3a-ae4d-82eca741b407", "admin@gmail.com", false, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEGUT6wzbnhgd/61RNp5asB0TfA84H5myAgubQkX5XnNx06fJC6AgiAL0TPkeBVIxxg==", null, false, 0.0, "75fc3822-999f-4da6-a3bb-823898269c08", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0ab43ecd-2a41-4900-82b2-62cb0e6a6414", "9f5325be-d52d-4519-aa0e-395ea11fe138" });
        }
    }
}

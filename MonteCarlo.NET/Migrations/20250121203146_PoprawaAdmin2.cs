using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class PoprawaAdmin2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { "1a321181-e0bb-40b0-ab1b-7451b5bee9e1", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b994b8d9-6850-48ca-9362-4400f5d11c6b", 0, "4e53d2f5-dc5c-4100-b65d-c422d649ff1a", "admin@gmail.com", true, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEJ1wFrBeVjwErAS4RSbhYGL8+tNr+YIgT0AwRLOUqg0SCkMDuyvhNfJkRXRrhe/+HQ==", null, false, 0.0, "0f64c5e1-1c46-4d3e-a7f4-401369d7fa5c", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1a321181-e0bb-40b0-ab1b-7451b5bee9e1", "b994b8d9-6850-48ca-9362-4400f5d11c6b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1a321181-e0bb-40b0-ab1b-7451b5bee9e1", "b994b8d9-6850-48ca-9362-4400f5d11c6b" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a321181-e0bb-40b0-ab1b-7451b5bee9e1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b994b8d9-6850-48ca-9362-4400f5d11c6b");

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
    }
}

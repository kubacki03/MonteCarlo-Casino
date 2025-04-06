using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class newMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c483665d-4392-43a8-a590-7c0dc482d29a", "ba70459a-1412-4bc6-8676-9b80f3e64817" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c483665d-4392-43a8-a590-7c0dc482d29a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ba70459a-1412-4bc6-8676-9b80f3e64817");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f09b998c-bc8e-46b8-b160-f752f7fc25b8", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ad445ab1-3c72-49ec-ae3a-8a1d75731887", 0, "a232b676-1a7c-41c1-a77d-1836b5c7d794", "admin@gmail.com", false, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEFitMzHr2ey+UgqlgN+z6AqBO5/u5eN3XY6uljU2UA2v3IhqE0mSsRVbYePrrsDtXQ==", null, false, 0.0, "1083f5fe-608b-4247-b6a4-b49e980c5da1", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "f09b998c-bc8e-46b8-b160-f752f7fc25b8", "ad445ab1-3c72-49ec-ae3a-8a1d75731887" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f09b998c-bc8e-46b8-b160-f752f7fc25b8", "ad445ab1-3c72-49ec-ae3a-8a1d75731887" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f09b998c-bc8e-46b8-b160-f752f7fc25b8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ad445ab1-3c72-49ec-ae3a-8a1d75731887");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c483665d-4392-43a8-a590-7c0dc482d29a", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ba70459a-1412-4bc6-8676-9b80f3e64817", 0, "4f13a96b-3941-44eb-b534-e63e68ba5546", "admin@gmail.com", false, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEBVGAq3lA4GqZ3uinsn7p/okqpBRtmSwpHxl41hHgtPaqhBIBsZR3mnDimZUZx/pmQ==", null, false, 0.0, "69f3e650-3681-41a6-a303-4857fd13f41c", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c483665d-4392-43a8-a590-7c0dc482d29a", "ba70459a-1412-4bc6-8676-9b80f3e64817" });
        }
    }
}

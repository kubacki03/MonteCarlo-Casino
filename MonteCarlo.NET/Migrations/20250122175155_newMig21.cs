using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class newMig21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}

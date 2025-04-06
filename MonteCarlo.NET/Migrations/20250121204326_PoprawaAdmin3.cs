using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class PoprawaAdmin3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { "94d17dcb-9e64-4f3a-9d59-1c3e7a061187", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Level", "LockoutEnabled", "LockoutEnd", "Nazwisko", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Saldo", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f2a541ba-4362-4293-ac46-544462370ddc", 0, "666fbea0-065c-4119-9297-ab372056630c", "admin@gmail.com", false, "Admin", 0, true, null, "Admin", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAECQ8PpDT0cKaZOfZZEQJEJ2aw7raBqolgyv90xOMfP/KmpNOX3l40snFbFx9o1FxdA==", null, false, 0.0, "40fda3a3-866f-4663-b938-5f07fc9a62b4", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "94d17dcb-9e64-4f3a-9d59-1c3e7a061187", "f2a541ba-4362-4293-ac46-544462370ddc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}

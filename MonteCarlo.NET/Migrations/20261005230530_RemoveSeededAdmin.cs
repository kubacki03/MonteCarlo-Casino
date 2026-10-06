using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeededAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "e61ff799-4f47-4940-ab43-e2cfe387334f", "8ca8f07b-ab7c-488c-b3bc-1571e10100da" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e61ff799-4f47-4940-ab43-e2cfe387334f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8ca8f07b-ab7c-488c-b3bc-1571e10100da");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e61ff799-4f47-4940-ab43-e2cfe387334f", "asd1", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Saldo", "ConcurrencyStamp", "Email", "EmailConfirmed", "Imie", "Nazwisko", "Level", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "8ca8f07b-ab7c-488c-b3bc-1571e10100da", 0, 0.0, "6b84402b-98b1-49d9-a10a-7a407cb1aa31", "admin@gmail.com", false, "Admin", "Admin", 0, true, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEFxm56pT3lkZSkHT1RKVEG2AWclz4TGAj89Tc4CAkgZrXfRtxC/6zj0+0T6F1UdPvA==", null, false, "115778a3-6593-4ad3-a680-8c83d11a9f80", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "e61ff799-4f47-4940-ab43-e2cfe387334f", "8ca8f07b-ab7c-488c-b3bc-1571e10100da" });
        }
    }
}

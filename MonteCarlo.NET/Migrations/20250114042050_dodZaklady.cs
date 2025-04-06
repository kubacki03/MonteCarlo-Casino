using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonteCarlo.NET.Migrations
{
    /// <inheritdoc />
    public partial class dodZaklady : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zaklady",
                columns: table => new
                {
                    IdZakladu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdGracza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdMeczu = table.Column<int>(type: "int", nullable: false),
                    IdZwyciezcy = table.Column<int>(type: "int", nullable: false),
                    PostawionaKwota = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaklady", x => x.IdZakladu);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zaklady");
        }
    }
}

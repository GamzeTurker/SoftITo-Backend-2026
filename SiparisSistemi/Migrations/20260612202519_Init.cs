using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiparisSistemi.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Musterilers",
                columns: table => new
                {
                    MusteriId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusteriAdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musterilers", x => x.MusteriId);
                });

            migrationBuilder.CreateTable(
                name: "Urunlers",
                columns: table => new
                {
                    UrunId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stok = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunlers", x => x.UrunId);
                });

            migrationBuilder.CreateTable(
                name: "Siparislers",
                columns: table => new
                {
                    SiparisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusteriId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Adet = table.Column<int>(type: "int", nullable: false),
                    SiparisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SiparisDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparislers", x => x.SiparisId);
                    table.ForeignKey(
                        name: "FK_Siparislers_Musterilers_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musterilers",
                        principalColumn: "MusteriId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Siparislers_Urunlers_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunlers",
                        principalColumn: "UrunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Siparislers_MusteriId",
                table: "Siparislers",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparislers_UrunId",
                table: "Siparislers",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Siparislers");

            migrationBuilder.DropTable(
                name: "Musterilers");

            migrationBuilder.DropTable(
                name: "Urunlers");
        }
    }
}

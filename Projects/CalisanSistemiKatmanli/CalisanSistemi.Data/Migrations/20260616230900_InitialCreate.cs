using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalisanSistemi.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departmans",
                columns: table => new
                {
                    DepartmanNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmanAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalisanSayisi = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departmans", x => x.DepartmanNo);
                });

            migrationBuilder.CreateTable(
                name: "GorevTipis",
                columns: table => new
                {
                    GorevTipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GorevTipis", x => x.GorevTipId);
                });

            migrationBuilder.CreateTable(
                name: "Personels",
                columns: table => new
                {
                    PersonelNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonelAdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Maas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Yas = table.Column<int>(type: "int", nullable: false),
                    Meslek = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DepartmanNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personels", x => x.PersonelNo);
                    table.ForeignKey(
                        name: "FK_Personels_Departmans_DepartmanNo",
                        column: x => x.DepartmanNo,
                        principalTable: "Departmans",
                        principalColumn: "DepartmanNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gorevs",
                columns: table => new
                {
                    GorevId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GorevTipId = table.Column<int>(type: "int", nullable: false),
                    TamamlandiMi = table.Column<bool>(type: "bit", nullable: false),
                    SonTarih = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PersonelNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gorevs", x => x.GorevId);
                    table.ForeignKey(
                        name: "FK_Gorevs_GorevTipis_GorevTipId",
                        column: x => x.GorevTipId,
                        principalTable: "GorevTipis",
                        principalColumn: "GorevTipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gorevs_Personels_PersonelNo",
                        column: x => x.PersonelNo,
                        principalTable: "Personels",
                        principalColumn: "PersonelNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gorevs_GorevTipId",
                table: "Gorevs",
                column: "GorevTipId");

            migrationBuilder.CreateIndex(
                name: "IX_Gorevs_PersonelNo",
                table: "Gorevs",
                column: "PersonelNo");

            migrationBuilder.CreateIndex(
                name: "IX_Personels_DepartmanNo",
                table: "Personels",
                column: "DepartmanNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Gorevs");

            migrationBuilder.DropTable(
                name: "GorevTipis");

            migrationBuilder.DropTable(
                name: "Personels");

            migrationBuilder.DropTable(
                name: "Departmans");
        }
    }
}

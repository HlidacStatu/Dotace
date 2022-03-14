using System;
using System.Collections.Generic;
using Common.IntermediateDb;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "exportready");

            migrationBuilder.CreateTable(
                name: "dotace",
                schema: "exportready",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    iddotace = table.Column<string>(type: "text", nullable: false),
                    datumpodpisu = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    datumaktualizace = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    kodprojektu = table.Column<string>(type: "text", nullable: true),
                    nazevprojektu = table.Column<string>(type: "text", nullable: true),
                    duplicita = table.Column<string>(type: "text", nullable: true),
                    rozhodnuti = table.Column<List<Rozhodnuti>>(type: "jsonb", nullable: false),
                    chyba = table.Column<List<string>>(type: "jsonb", nullable: true),
                    programnazev = table.Column<string>(type: "text", nullable: true),
                    programkod = table.Column<string>(type: "text", nullable: true),
                    prijemceico = table.Column<string>(type: "text", nullable: true),
                    prijemceobchodnijmeno = table.Column<string>(type: "text", nullable: true),
                    prijemcehlidacjmeno = table.Column<string>(type: "text", nullable: true),
                    prijemcejmeno = table.Column<string>(type: "text", nullable: true),
                    prijemceroknarozeni = table.Column<int>(type: "integer", nullable: true),
                    prijemceobec = table.Column<string>(type: "text", nullable: true),
                    prijemceokres = table.Column<string>(type: "text", nullable: true),
                    prijemcepsc = table.Column<string>(type: "text", nullable: true),
                    prijemceulice = table.Column<string>(type: "text", nullable: true),
                    zdrojnazev = table.Column<string>(type: "text", nullable: false),
                    zdrojurl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dotace", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dotace",
                schema: "exportready");
        }
    }
}

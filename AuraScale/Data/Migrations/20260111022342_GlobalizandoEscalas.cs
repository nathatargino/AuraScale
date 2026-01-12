using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraScale.Data.Migrations
{
    /// <inheritdoc />
    public partial class GlobalizandoEscalas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Turno",
                table: "Escalas",
                newName: "Saida");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HorarioEntrada",
                table: "Operadores",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "ModeloEscalaId",
                table: "Operadores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Entrada",
                table: "Escalas",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "GerenteId",
                table: "Escalas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Escalas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModelosEscala",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    CargaHorariaDiaria = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TrabalhaSabado = table.Column<bool>(type: "INTEGER", nullable: false),
                    CargaHorariaSabado = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    TrabalhaDomingo = table.Column<bool>(type: "INTEGER", nullable: false),
                    GerenteId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosEscala", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelosEscala_AspNetUsers_GerenteId",
                        column: x => x.GerenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Operadores_ModeloEscalaId",
                table: "Operadores",
                column: "ModeloEscalaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosEscala_GerenteId",
                table: "ModelosEscala",
                column: "GerenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operadores_ModelosEscala_ModeloEscalaId",
                table: "Operadores",
                column: "ModeloEscalaId",
                principalTable: "ModelosEscala",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operadores_ModelosEscala_ModeloEscalaId",
                table: "Operadores");

            migrationBuilder.DropTable(
                name: "ModelosEscala");

            migrationBuilder.DropIndex(
                name: "IX_Operadores_ModeloEscalaId",
                table: "Operadores");

            migrationBuilder.DropColumn(
                name: "HorarioEntrada",
                table: "Operadores");

            migrationBuilder.DropColumn(
                name: "ModeloEscalaId",
                table: "Operadores");

            migrationBuilder.DropColumn(
                name: "Entrada",
                table: "Escalas");

            migrationBuilder.DropColumn(
                name: "GerenteId",
                table: "Escalas");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Escalas");

            migrationBuilder.RenameColumn(
                name: "Saida",
                table: "Escalas",
                newName: "Turno");
        }
    }
}

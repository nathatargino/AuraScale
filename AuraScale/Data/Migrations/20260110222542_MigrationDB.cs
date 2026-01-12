using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraScale.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operadores_AspNetUsers_GerenteId",
                table: "Operadores");

            migrationBuilder.AlterColumn<string>(
                name: "GerenteId",
                table: "Operadores",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Funcao",
                table: "Operadores",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Operadores_AspNetUsers_GerenteId",
                table: "Operadores",
                column: "GerenteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operadores_AspNetUsers_GerenteId",
                table: "Operadores");

            migrationBuilder.AlterColumn<string>(
                name: "GerenteId",
                table: "Operadores",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Funcao",
                table: "Operadores",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Operadores_AspNetUsers_GerenteId",
                table: "Operadores",
                column: "GerenteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

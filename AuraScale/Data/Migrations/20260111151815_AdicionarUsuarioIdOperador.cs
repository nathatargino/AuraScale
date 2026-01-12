using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraScale.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUsuarioIdOperador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Operadores",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Operadores");
        }
    }
}

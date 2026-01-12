using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraScale.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarRegraSabadoEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrabalhaSabado",
                table: "ModelosEscala",
                newName: "RegraSabado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegraSabado",
                table: "ModelosEscala",
                newName: "TrabalhaSabado");
        }
    }
}

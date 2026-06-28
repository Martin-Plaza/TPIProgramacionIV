using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiControl.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioResponsable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUsuarioResponsable",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdUsuarioResponsable",
                table: "Usuarios",
                column: "IdUsuarioResponsable");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioResponsable",
                table: "Usuarios",
                column: "IdUsuarioResponsable",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_IdUsuarioResponsable",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdUsuarioResponsable",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IdUsuarioResponsable",
                table: "Usuarios");
        }
    }
}

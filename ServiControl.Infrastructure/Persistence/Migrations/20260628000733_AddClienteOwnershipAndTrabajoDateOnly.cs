using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiControl.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteOwnershipAndTrabajoDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Fecha",
                table: "Trabajos",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                DECLARE @UsuarioId int;
                SELECT TOP(1) @UsuarioId = [Id] FROM [Usuarios] ORDER BY [Id];

                IF EXISTS (SELECT 1 FROM [Clientes]) AND @UsuarioId IS NULL
                BEGIN
                    THROW 50001, 'No se puede migrar Clientes.UsuarioId porque existen clientes y no hay usuarios para asignarlos.', 1;
                END

                UPDATE cliente
                SET [UsuarioId] = trabajo.[UsuarioId]
                FROM [Clientes] cliente
                CROSS APPLY (
                    SELECT TOP(1) [UsuarioId]
                    FROM [Trabajos]
                    WHERE [ClienteId] = cliente.[Id]
                    ORDER BY [Id]
                ) trabajo
                WHERE cliente.[UsuarioId] IS NULL;

                UPDATE [Clientes]
                SET [UsuarioId] = @UsuarioId
                WHERE [UsuarioId] IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Clientes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Clientes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Trabajos",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}

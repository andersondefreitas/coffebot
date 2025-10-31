using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatbotApiNova.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoRelacaoComentarioUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Usuarios_UsuarioCriadorIdUsuario",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_UsuarioCriadorIdUsuario",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioCriadorIdUsuario",
                table: "Comentarios");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_IdUsuarioCriador",
                table: "Comentarios",
                column: "IdUsuarioCriador");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Usuarios_IdUsuarioCriador",
                table: "Comentarios",
                column: "IdUsuarioCriador",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Usuarios_IdUsuarioCriador",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_IdUsuarioCriador",
                table: "Comentarios");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCriadorIdUsuario",
                table: "Comentarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioCriadorIdUsuario",
                table: "Comentarios",
                column: "UsuarioCriadorIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Usuarios_UsuarioCriadorIdUsuario",
                table: "Comentarios",
                column: "UsuarioCriadorIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

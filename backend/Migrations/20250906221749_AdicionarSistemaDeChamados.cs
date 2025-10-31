using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatbotApiNova.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarSistemaDeChamados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Tecnicos_TecnicoResponsavelIdTecnico",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Usuarios_UsuarioAberturaIdUsuario",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Chamados_ChamadoIdChamado",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Tecnicos_Usuarios_UsuarioIdUsuario",
                table: "Tecnicos");

            migrationBuilder.DropIndex(
                name: "IX_Tecnicos_UsuarioIdUsuario",
                table: "Tecnicos");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_ChamadoIdChamado",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_TecnicoResponsavelIdTecnico",
                table: "Chamados");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_UsuarioAberturaIdUsuario",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Tecnicos");

            migrationBuilder.DropColumn(
                name: "ChamadoIdChamado",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "TecnicoResponsavelIdTecnico",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "UsuarioAberturaIdUsuario",
                table: "Chamados");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_IdUsuario",
                table: "Tecnicos",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_IdChamado",
                table: "Comentarios",
                column: "IdChamado");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_IdTecnicoResponsavel",
                table: "Chamados",
                column: "IdTecnicoResponsavel");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_IdUsuarioAbertura",
                table: "Chamados",
                column: "IdUsuarioAbertura");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Tecnicos_IdTecnicoResponsavel",
                table: "Chamados",
                column: "IdTecnicoResponsavel",
                principalTable: "Tecnicos",
                principalColumn: "IdTecnico",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Usuarios_IdUsuarioAbertura",
                table: "Chamados",
                column: "IdUsuarioAbertura",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Chamados_IdChamado",
                table: "Comentarios",
                column: "IdChamado",
                principalTable: "Chamados",
                principalColumn: "IdChamado",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tecnicos_Usuarios_IdUsuario",
                table: "Tecnicos",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Tecnicos_IdTecnicoResponsavel",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Usuarios_IdUsuarioAbertura",
                table: "Chamados");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Chamados_IdChamado",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Tecnicos_Usuarios_IdUsuario",
                table: "Tecnicos");

            migrationBuilder.DropIndex(
                name: "IX_Tecnicos_IdUsuario",
                table: "Tecnicos");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_IdChamado",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_IdTecnicoResponsavel",
                table: "Chamados");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_IdUsuarioAbertura",
                table: "Chamados");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Tecnicos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChamadoIdChamado",
                table: "Comentarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TecnicoResponsavelIdTecnico",
                table: "Chamados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioAberturaIdUsuario",
                table: "Chamados",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_UsuarioIdUsuario",
                table: "Tecnicos",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ChamadoIdChamado",
                table: "Comentarios",
                column: "ChamadoIdChamado");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_TecnicoResponsavelIdTecnico",
                table: "Chamados",
                column: "TecnicoResponsavelIdTecnico");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_UsuarioAberturaIdUsuario",
                table: "Chamados",
                column: "UsuarioAberturaIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Tecnicos_TecnicoResponsavelIdTecnico",
                table: "Chamados",
                column: "TecnicoResponsavelIdTecnico",
                principalTable: "Tecnicos",
                principalColumn: "IdTecnico");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Usuarios_UsuarioAberturaIdUsuario",
                table: "Chamados",
                column: "UsuarioAberturaIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Chamados_ChamadoIdChamado",
                table: "Comentarios",
                column: "ChamadoIdChamado",
                principalTable: "Chamados",
                principalColumn: "IdChamado",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tecnicos_Usuarios_UsuarioIdUsuario",
                table: "Tecnicos",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatbotApiNova.Migrations
{
    /// <inheritdoc />
    public partial class VersaoFinalDoBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    TipoUsuario = table.Column<string>(type: "text", nullable: false),
                    EmailConfirmado = table.Column<bool>(type: "boolean", nullable: false),
                    TokenConfirmacaoEmail = table.Column<string>(type: "text", nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "text", nullable: true),
                    ResetPasswordTokenExpires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    IdTecnico = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    Especialidade = table.Column<string>(type: "text", nullable: true),
                    Disponivel = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.IdTecnico);
                    table.ForeignKey(
                        name: "FK_Tecnicos_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chamados",
                columns: table => new
                {
                    IdChamado = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Prioridade = table.Column<string>(type: "text", nullable: false),
                    IdUsuarioAbertura = table.Column<int>(type: "integer", nullable: false),
                    IdTecnicoResponsavel = table.Column<int>(type: "integer", nullable: true),
                    UsuarioAberturaIdUsuario = table.Column<int>(type: "integer", nullable: false),
                    TecnicoResponsavelIdTecnico = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chamados", x => x.IdChamado);
                    table.ForeignKey(
                        name: "FK_Chamados_Tecnicos_TecnicoResponsavelIdTecnico",
                        column: x => x.TecnicoResponsavelIdTecnico,
                        principalTable: "Tecnicos",
                        principalColumn: "IdTecnico");
                    table.ForeignKey(
                        name: "FK_Chamados_Usuarios_UsuarioAberturaIdUsuario",
                        column: x => x.UsuarioAberturaIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    IdComentario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Conteudo = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdChamado = table.Column<int>(type: "integer", nullable: false),
                    IdUsuarioCriador = table.Column<int>(type: "integer", nullable: false),
                    ChamadoIdChamado = table.Column<int>(type: "integer", nullable: false),
                    UsuarioCriadorIdUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.IdComentario);
                    table.ForeignKey(
                        name: "FK_Comentarios_Chamados_ChamadoIdChamado",
                        column: x => x.ChamadoIdChamado,
                        principalTable: "Chamados",
                        principalColumn: "IdChamado",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_UsuarioCriadorIdUsuario",
                        column: x => x.UsuarioCriadorIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_TecnicoResponsavelIdTecnico",
                table: "Chamados",
                column: "TecnicoResponsavelIdTecnico");

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_UsuarioAberturaIdUsuario",
                table: "Chamados",
                column: "UsuarioAberturaIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ChamadoIdChamado",
                table: "Comentarios",
                column: "ChamadoIdChamado");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioCriadorIdUsuario",
                table: "Comentarios",
                column: "UsuarioCriadorIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_UsuarioIdUsuario",
                table: "Tecnicos",
                column: "UsuarioIdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Chamados");

            migrationBuilder.DropTable(
                name: "Tecnicos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}

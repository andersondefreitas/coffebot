using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatbotApiNova.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenConfirmacaoEmailExpiracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokenConfirmacaoEmailExpiracao",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenConfirmacaoEmailExpiracao",
                table: "Usuarios");
        }
    }
}

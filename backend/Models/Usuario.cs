using System.ComponentModel.DataAnnotations;
using System; // Adicionado para DateTime

namespace ChatbotApi.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public required string Nome { get; set; }

        public required string Email { get; set; }

        public required string Senha { get; set; }

        public required string TipoUsuario { get; set; }

        public bool EmailConfirmado { get; set; } = false; 
        public string? TokenConfirmacaoEmail { get; set; }

        // --- LINHA ADICIONADA ---
        public DateTime? TokenConfirmacaoEmailExpiracao { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpires { get; set; }
    }
}
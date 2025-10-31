// Models/Tecnico.cs
using System.ComponentModel.DataAnnotations;

namespace ChatbotApi.Models
{
    public class Tecnico
    {
        [Key]
        public int IdTecnico { get; set; }
        public int IdUsuario { get; set; } // Chave estrangeira
        public string? Especialidade { get; set; }
        public bool Disponivel { get; set; } = true;

        // Propriedade de navegação para o usuário
        public Usuario Usuario { get; set; } = null!;
    }
}
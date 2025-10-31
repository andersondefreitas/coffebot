// Models/Comentario.cs
using System.ComponentModel.DataAnnotations;

namespace ChatbotApi.Models
{
    public class Comentario
    {
        [Key]
        public int IdComentario { get; set; }
        public required string Conteudo { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Chaves estrangeiras
        public int IdChamado { get; set; }
        public int IdUsuarioCriador { get; set; }

        // Propriedades de navegação
        public Chamado Chamado { get; set; } = null!;
        public Usuario UsuarioCriador { get; set; } = null!;
    }
}
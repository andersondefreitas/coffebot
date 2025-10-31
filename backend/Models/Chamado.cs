using System.ComponentModel.DataAnnotations;

namespace ChatbotApi.Models
{
    public class Chamado
    {
        [Key]
        public int IdChamado { get; set; }
        public required string Titulo { get; set; }
        public required string Descricao { get; set; }
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public DateTime? DataFechamento { get; set; }
        public required string Status { get; set; }
        public required string Prioridade { get; set; }

        // Chaves estrangeiras
        public int IdUsuarioAbertura { get; set; }
        public int? IdTecnicoResponsavel { get; set; }

        // Propriedades de navegação
        public Usuario UsuarioAbertura { get; set; } = null!;
        public Tecnico? TecnicoResponsavel { get; set; }
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
    }
}
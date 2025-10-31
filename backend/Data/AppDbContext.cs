using ChatbotApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotApi.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionamento: Um Chamado tem muitos Comentários
            modelBuilder.Entity<Chamado>()
                .HasMany(c => c.Comentarios)
                .WithOne(cm => cm.Chamado)
                .HasForeignKey(cm => cm.IdChamado)
                .OnDelete(DeleteBehavior.Cascade); 

            // Relacionamento: Um Usuário pode abrir muitos Chamados
            modelBuilder.Entity<Usuario>()
                .HasMany<Chamado>() 
                .WithOne(c => c.UsuarioAbertura)
                .HasForeignKey(c => c.IdUsuarioAbertura)
                .OnDelete(DeleteBehavior.Restrict); 

            // Relacionamento: Um Técnico é um Usuário (One-to-One)
            modelBuilder.Entity<Tecnico>()
                .HasOne(t => t.Usuario)
                .WithOne() 
                .HasForeignKey<Tecnico>(t => t.IdUsuario);

            // Relacionamento: Um Técnico pode ser responsável por muitos Chamados
            modelBuilder.Entity<Tecnico>()
                .HasMany<Chamado>()
                .WithOne(c => c.TecnicoResponsavel)
                .HasForeignKey(c => c.IdTecnicoResponsavel)
                .IsRequired(false) 
                .OnDelete(DeleteBehavior.SetNull); 

            
            // --- ADICIONADO: Relacionamento entre Comentário e Usuário ---
            // Isto corrige o erro 400 Bad Request
            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.UsuarioCriador) // O comentário tem um UsuarioCriador
                .WithMany() // Um Usuário pode ter muitos comentários (não precisamos mapear o inverso)
                .HasForeignKey(c => c.IdUsuarioCriador) // A chave estrangeira é IdUsuarioCriador
                .OnDelete(DeleteBehavior.Restrict); // Impede que um usuário seja deletado se tiver comentários
        }
    }
}
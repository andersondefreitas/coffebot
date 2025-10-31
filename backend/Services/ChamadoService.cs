// Local: Services/ChamadoService.cs
using ChatbotApi.Data;
using ChatbotApi.Models;
using Microsoft.EntityFrameworkCore;
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;

namespace ChatbotApi.Services
{
    public class ChamadoService : IChamadoService
    {
        private readonly AppDbContext _context;

        public ChamadoService(AppDbContext context)
        {
            _context = context;
        }

        // --- CORRIGIDO ---
        // Adiciona 'Prioridade = "A definir"' para corrigir o erro CS9035
        public async Task<Chamado> AbrirChamadoAsync(int usuarioId, string titulo, string descricao)
        {
            var novoChamado = new Chamado
            {
                Titulo = titulo,
                Descricao = descricao,
                IdUsuarioAbertura = usuarioId,
                DataAbertura = DateTime.UtcNow,
                Status = "A definir prioridade", 
                Prioridade = "A definir" // <-- ESTA LINHA CORRIGE O ERRO
            };

            await _context.Chamados.AddAsync(novoChamado);
            await _context.SaveChangesAsync();
            return novoChamado;
        }

        // --- Método ListarChamados (com filtro de Arquivado) ---
        public async Task<IEnumerable<Chamado>> ListarChamadosAsync()
        {
            return await _context.Chamados
                .Where(c => c.Status != "Arquivado") 
                .Include(c => c.UsuarioAbertura) 
                .Include(c => c.TecnicoResponsavel) 
                    .ThenInclude(t => t!.Usuario) 
                .OrderByDescending(c => c.DataAbertura)
                .ToListAsync();
        }

        // --- Método AtualizarStatus (com lógica de Arquivado) ---
        public async Task<bool> AtualizarStatusChamadoAsync(int chamadoId, string novoStatus)
        {
            var chamado = await _context.Chamados.FindAsync(chamadoId);
            if (chamado == null) return false;

            chamado.Status = novoStatus;
            
            if (novoStatus == "Suporte completo" || novoStatus == "Arquivado")
            {
                chamado.DataFechamento = DateTime.UtcNow;
            }
            else
            {
                chamado.DataFechamento = null;
            }

            try
            {
                var linhasAfetadas = await _context.SaveChangesAsync();
                return linhasAfetadas > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        // --- Método para LISTAR Comentários ---
        public async Task<IEnumerable<Comentario>> ListarComentariosPorChamadoAsync(int chamadoId)
        {
            return await _context.Comentarios
                .Where(c => c.IdChamado == chamadoId)
                .Include(c => c.UsuarioCriador) 
                .OrderBy(c => c.DataCriacao) 
                .ToListAsync();
        }

        // --- Método para ADICIONAR Comentário ---
        public async Task<Comentario> AdicionarComentarioAsync(int chamadoId, int usuarioId, string texto)
        {
            var novoComentario = new Comentario
            {
                IdChamado = chamadoId,
                IdUsuarioCriador = usuarioId, 
                Conteudo = texto,          
                DataCriacao = DateTime.UtcNow  
            };

            await _context.Comentarios.AddAsync(novoComentario);
            await _context.SaveChangesAsync();

            await _context.Entry(novoComentario)
                .Reference(c => c.UsuarioCriador) 
                .LoadAsync();
            
            return novoComentario;
        }

        // --- Método para DEFINIR PRIORIDADE ---
        public async Task<Chamado?> DefinirPrioridadeAsync(int chamadoId, string novaPrioridade)
        {
            var chamado = await _context.Chamados.FindAsync(chamadoId);
            if (chamado == null)
            {
                return null; 
            }

            string novoStatus = MapearPrioridadeParaStatus(novaPrioridade);
            chamado.Prioridade = novaPrioridade;
            chamado.Status = novoStatus;

            try
            {
                await _context.SaveChangesAsync();
                // Recarregamos todas as referências para o frontend
                await _context.Entry(chamado).Reference(c => c.UsuarioAbertura).LoadAsync();
                await _context.Entry(chamado).Reference(c => c.TecnicoResponsavel).LoadAsync();
                if(chamado.TecnicoResponsavel != null)
                {
                    await _context.Entry(chamado.TecnicoResponsavel).Reference(t => t.Usuario).LoadAsync();
                }
                return chamado;
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }
        
        // --- Método para ATRIBUIR CHAMADO ---
        public async Task<Chamado?> AtribuirChamadoAsync(int chamadoId, int usuarioId)
        {
            var chamado = await _context.Chamados.FindAsync(chamadoId);
            if (chamado == null)
            {
                return null; // Chamado não encontrado
            }

            var tecnico = await _context.Tecnicos
                                    .Include(t => t.Usuario) 
                                    .FirstOrDefaultAsync(t => t.IdUsuario == usuarioId);
            
            if (tecnico == null)
            {
                return null; // Este usuário não é um técnico
            }

            chamado.IdTecnicoResponsavel = tecnico.IdTecnico;
            chamado.Status = "Suporte em andamento";

            try
            {
                await _context.SaveChangesAsync();
                await _context.Entry(chamado).Reference(c => c.UsuarioAbertura).LoadAsync();
                return chamado;
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }

        // --- Método auxiliar (sem alteração) ---
        private string MapearPrioridadeParaStatus(string prioridade)
        {
            return prioridade.ToLower() switch
            {
                "critica" => "Prioridade: critica",
                "média" => "Prioridade: média",
                "baixa" => "Prioridade: baixa",
                _ => "A definir prioridade"
            };
        }
    }
}
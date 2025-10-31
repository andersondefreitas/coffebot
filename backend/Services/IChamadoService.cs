// Local: Services/IChamadoService.cs
using ChatbotApi.Models;
using System.Threading.Tasks; 
using System.Collections.Generic; 

namespace ChatbotApi.Services
{
    public interface IChamadoService
    {
        Task<Chamado> AbrirChamadoAsync(int usuarioId, string titulo, string descricao);
        Task<IEnumerable<Chamado>> ListarChamadosAsync();
        Task<bool> AtualizarStatusChamadoAsync(int chamadoId, string novoStatus);

        // --- Histórico de Comentários ---
        Task<IEnumerable<Comentario>> ListarComentariosPorChamadoAsync(int chamadoId);
        Task<Comentario> AdicionarComentarioAsync(int chamadoId, int usuarioId, string texto);
        
        // --- Definir Prioridade ---
        Task<Chamado?> DefinirPrioridadeAsync(int chamadoId, string novaPrioridade);

        // --- ADICIONADO PARA ATRIBUIR RESPONSÁVEL ---
        Task<Chamado?> AtribuirChamadoAsync(int chamadoId, int usuarioId);
    }
}
// Local: Controllers/ChamadoController.cs
using ChatbotApi.Models;
using ChatbotApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatbotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // <-- Exige login para TUDO
    public class ChamadoController : ControllerBase
    {
        private readonly IChamadoService _chamadoService;

        public ChamadoController(IChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        // --- DTOs (Data Transfer Objects) ---
        public class AbrirChamadoDto
        {
            public required string Titulo { get; set; }
            public required string Descricao { get; set; }
        }
        public class AtualizarStatusDto
        {
            public required string NovoStatus { get; set; }
        }
        public class AdicionarComentarioDto
        {
            public required string Texto { get; set; }
        }
        public class DefinirPrioridadeDto
        {
            public required string Prioridade { get; set; }
        }
        // ------------------------------------

        // --- POST /api/chamado ---
        [HttpPost]
        public async Task<IActionResult> AbrirChamado([FromBody] AbrirChamadoDto chamadoDto)
        {
            var idUsuarioString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUsuarioString == null) return Unauthorized();
            var idUsuario = int.Parse(idUsuarioString);

            try
            {
                var novoChamado = await _chamadoService.AbrirChamadoAsync(idUsuario, chamadoDto.Titulo, chamadoDto.Descricao);
                return Ok(novoChamado);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao abrir chamado: {ex.Message}" });
            }
        }

        // --- GET /api/chamado ---
        [HttpGet]
        [Authorize(Policy = "AdminOuTecnico")] 
        public async Task<IActionResult> ListarChamados()
        {
            try
            {
                var chamados = await _chamadoService.ListarChamadosAsync();
                return Ok(chamados);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao listar chamados: {ex.Message}" });
            }
        }

        // --- PATCH /api/chamado/{idChamado} ---
        [HttpPatch("{idChamado}")]
        [Authorize(Policy = "AdminOuTecnico")] 
        public async Task<IActionResult> AtualizarStatus(
            int idChamado, 
            [FromBody] AtualizarStatusDto dto)
        {
            try
            {
                var sucesso = await _chamadoService.AtualizarStatusChamadoAsync(idChamado, dto.NovoStatus);
                if (sucesso)
                {
                    return Ok(new { mensagem = "Status atualizado com sucesso." });
                }
                else
                {
                    return NotFound(new { mensagem = "Chamado não encontrado." });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao atualizar status: {ex.Message}" });
            }
        }
        
        // --- GET /api/chamado/{idChamado}/comentarios ---
        [HttpGet("{idChamado}/comentarios")]
        public async Task<IActionResult> ListarComentarios(int idChamado)
        {
            try
            {
                var comentarios = await _chamadoService.ListarComentariosPorChamadoAsync(idChamado);
                return Ok(comentarios);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao listar comentários: {ex.Message}" });
            }
        }
        
        // --- POST /api/chamado/{idChamado}/comentarios ---
        [HttpPost("{idChamado}/comentarios")]
        public async Task<IActionResult> AdicionarComentario(
            int idChamado, 
            [FromBody] AdicionarComentarioDto dto)
        {
            var idUsuarioString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUsuarioString == null) return Unauthorized();
            var idUsuario = int.Parse(idUsuarioString);

            try
            {
                var novoComentario = await _chamadoService.AdicionarComentarioAsync(idChamado, idUsuario, dto.Texto);
                return Ok(novoComentario);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao adicionar comentário: {ex.Message}" });
            }
        }

        // --- PATCH /api/chamado/{idChamado}/prioridade ---
        [HttpPatch("{idChamado}/prioridade")]
        [Authorize(Policy = "AdminOuTecnico")] 
        public async Task<IActionResult> DefinirPrioridade(
            int idChamado,
            [FromBody] DefinirPrioridadeDto dto)
        {
            try
            {
                var chamadoAtualizado = await _chamadoService.DefinirPrioridadeAsync(idChamado, dto.Prioridade);
                if (chamadoAtualizado == null)
                {
                    return NotFound(new { mensagem = "Chamado não encontrado." });
                }
                return Ok(chamadoAtualizado);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao definir prioridade: {ex.Message}" });
            }
        }

        // --- ADICIONADO: PATCH /api/chamado/{idChamado}/atribuir ---
        // (Para o modal "Assumir Chamado")
        [HttpPatch("{idChamado}/atribuir")]
        [Authorize(Policy = "AdminOuTecnico")] // Apenas Técnicos/Admins
        public async Task<IActionResult> AtribuirChamado(int idChamado)
        {
            // Pega o ID do técnico que está a clicar no botão (a partir do token)
            var idUsuarioString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUsuarioString == null) return Unauthorized();
            var idUsuario = int.Parse(idUsuarioString);

            try
            {
                // 1. Chama o serviço (que atribui o técnico E muda o status)
                var chamadoAtualizado = await _chamadoService.AtribuirChamadoAsync(idChamado, idUsuario);
                
                // 2. Verifica se foi encontrado
                if (chamadoAtualizado == null)
                {
                    // (Pode ser que o 'idChamado' esteja errado ou o 'idUsuario' não seja um técnico)
                    return NotFound(new { mensagem = "Chamado ou Técnico não encontrado." });
                }

                // 3. Retorna o chamado atualizado (com o técnico e o novo status)
                return Ok(chamadoAtualizado);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao atribuir chamado: {ex.Message}" });
            }
        }
        // --------------------------------------------------
    }
}
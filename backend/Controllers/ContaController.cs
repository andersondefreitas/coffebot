// Local: ChatbotApi/Controllers/ContaController.cs
using ChatbotApi.Data;
using ChatbotApi.Models;
using ChatbotApi.Services; 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System; // Adicionado para Exception e Console

namespace ChatbotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
    
        private readonly IContaService _contaService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        // NOTA: Para produção, injete IConfiguration para URLs e ILogger para logs.
        // private readonly IConfiguration _configuration;
        // private readonly ILogger<ContaController> _logger;

        public ContaController(
            IContaService contaService,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _contaService = contaService;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        // --- Modelos para receber dados do Front-end (DTOs) ---

        public class RegistroDto
        {
            public required string Nome { get; set; }
            public required string Email { get; set; }
            public required string Senha { get; set; }
        }

        public class LoginDto
        {
            public required string Email { get; set; }
            public required string Senha { get; set; }
        }

        public class AlterarSenhaDto
        {
            public required string SenhaAtual { get; set; }
            public required string NovaSenha { get; set; }
        }

        public class EsqueciSenhaDto
        {
            public required string Email { get; set; }
        }
        
        // NOVO DTO para confirmar com código
        public class ConfirmarEmailDto
        {
            public required string Email { get; set; }
            public required string Codigo { get; set; } // O código de 6 dígitos
        }

        // DTO MODIFICADO para redefinir com código
        public class RedefinirSenhaDto
        {
            public required string Email { get; set; } // Precisa do email para achar o usuário
            public required string Codigo { get; set; } // O código de 6 dígitos
            public required string NovaSenha { get; set; }
        }

        // --- Endpoints ---

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto dadosRegistro)
        {
            try
            {
                // Assumindo que RegisterAsync agora gera um código de 6 dígitos em 'TokenConfirmacaoEmail'
                var novoUsuario = await _contaService.RegisterAsync(dadosRegistro.Nome, dadosRegistro.Email, dadosRegistro.Senha);

                // [MUDANÇA] O corpo do email agora envia o código
                var corpoEmail = $"<h1>Bem-vindo ao Coffeebot!</h1><p>Seu código de confirmação é: <strong>{novoUsuario.TokenConfirmacaoEmail}</strong></p><p>Este código expira em 10 minutos.</p>";
                await _emailService.EnviarEmailAsync(novoUsuario.Email, "Confirme sua conta - Coffeebot", corpoEmail);
                
                return Ok(new { mensagem = "Usuário registrado com sucesso! Por favor, verifique seu email para obter seu código de confirmação." });
            }
            catch (Exception ex) // Idealmente, capturar exceções específicas (ex: EmailJaCadastradoException)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // [MUDANÇA] O endpoint mudou de [HttpGet] com token na query para [HttpPost] com DTO no body
        [HttpPost("confirmar-email")]
        public async Task<IActionResult> ConfirmarEmail([FromBody] ConfirmarEmailDto dados)
        {
            // Assumindo que ConfirmEmailAsync foi atualizado para receber (email, codigo)
            var sucesso = await _contaService.ConfirmEmailAsync(dados.Email, dados.Codigo);
            
            if (!sucesso)
            {
                return BadRequest("Email não encontrado ou código inválido/expirado.");
            }
            return Ok("Email confirmado com sucesso! Você já pode fazer login.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dadosLogin)
        {
            var usuario = await _contaService.ValidateCredentialsAsync(dadosLogin.Email, dadosLogin.Senha);
            if (usuario == null)
            {
                return Unauthorized("Email ou senha inválidos.");
            }

            if (!usuario.EmailConfirmado)
            {
                return Unauthorized("Sua conta ainda não foi confirmada. Por favor, verifique seu email.");
            }
            
            // Supondo que seu ITokenService tenha um método GerarTokenJwt
            var token = _tokenService.GerarTokenJwt(usuario);
            return Ok(new { token });
        }

        [HttpPost("alterar-senha")]
        [Authorize]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDto dados)
        {
            var idUsuarioString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idUsuarioString == null) return Unauthorized();

            var sucesso = await _contaService.ChangePasswordAsync(int.Parse(idUsuarioString), dados.SenhaAtual, dados.NovaSenha);
            if (!sucesso)
            {
                return BadRequest(new { mensagem = "A senha atual está incorreta ou o usuário não foi encontrado." });
            }

            return Ok(new { mensagem = "Senha alterada com sucesso." });
        }

        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaDto dados)
        {
            // Assumindo que InitiatePasswordResetAsync agora gera um código de 6 dígitos em 'ResetPasswordToken'
            var usuario = await _contaService.InitiatePasswordResetAsync(dados.Email);
            
            if (usuario != null && usuario.ResetPasswordToken != null)
            {
                // [MUDANÇA] O corpo do email agora envia o código
                var corpoEmail = $"<h1>Redefinição de Senha</h1><p>Seu código para redefinir a senha é: <strong>{usuario.ResetPasswordToken}</strong></p><p>Este código expira em 10 minutos.</p>";
                
                try
                {
                    await _emailService.EnviarEmailAsync(usuario.Email, "Redefinição de Senha - Coffeebot", corpoEmail);
                }
                catch (Exception ex)
                {
                    // TODO: Substituir por ILogger.LogError()
                    Console.WriteLine($"Erro ao enviar email de redefinição: {ex.Message}");
                }
            }
            
            // Mensagem genérica por segurança
            return Ok(new { mensagem = "Se o email estiver cadastrado, um código de redefinição foi enviado." });
        }

        // [MUDANÇA] O endpoint agora recebe o DTO modificado
        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDto dados)
        {
            // Assumindo que ResetPasswordAsync foi atualizado para receber (email, codigo, novaSenha)
            var sucesso = await _contaService.ResetPasswordAsync(dados.Email, dados.Codigo, dados.NovaSenha);
            
            if (!sucesso)
            {
                return BadRequest("Email/código inválido, expirado ou a senha não atende aos requisitos.");
            }
            return Ok(new { mensagem = "Senha redefinida com sucesso!" });
        }
    }
}
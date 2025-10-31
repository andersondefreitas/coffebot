// Local: Servicos/TokenService.cs
using ChatbotApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // <-- Importante para o ClaimTypes.Role
using System.Text;
// (IConfiguration também é necessário)
using Microsoft.Extensions.Configuration; 

namespace ChatbotApi.Services
{
    public interface ITokenService
    {
        string GerarTokenJwt(Usuario usuario);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuracao;

        public TokenService(IConfiguration configuracao)
        {
            _configuracao = configuracao;
        }

        public string GerarTokenJwt(Usuario usuario)
        {
            var jwtKey = _configuracao["Jwt:Key"] ?? throw new InvalidOperationException("Chave JWT não configurada.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // (O ID do usuário)
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                // (O Email)
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                // (O Nome)
                new Claim("nome", usuario.Nome),
                
                // --- MUDANÇA CRÍTICA AQUI ---
                // Trocamos "tipo_usuario" pelo padrão do .NET "ClaimTypes.Role"
                // Agora o [Authorize(Policy...)] vai funcionar.
                new Claim(ClaimTypes.Role, usuario.TipoUsuario)
            };

            var token = new JwtSecurityToken(
                issuer: _configuracao["Jwt:Issuer"],
                audience: _configuracao["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
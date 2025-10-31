// Local: ChatbotApi/Services/ContaServices.cs
using ChatbotApi.Data;
using ChatbotApi.Models;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System; // Adicionado para Random e DateTime

namespace ChatbotApi.Services
{
    // --- [MUDANÇA] Interface atualizada ---
    public interface IContaService
    {
        Task<Usuario> RegisterAsync(string nome, string email, string senha);
        // MUDOU: Recebe email/código e retorna bool
        Task<bool> ConfirmEmailAsync(string email, string codigo);
        Task<Usuario?> ValidateCredentialsAsync(string email, string senha);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<Usuario?> InitiatePasswordResetAsync(string email);
        // MUDOU: Recebe email/código/novaSenha e retorna bool
        Task<bool> ResetPasswordAsync(string email, string codigo, string newPassword);
    }

    public class ContaService : IContaService
    {
        private readonly AppDbContext _context;
        // Gerador de número aleatório para os códigos
        private static readonly Random _random = new Random();

        public ContaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> RegisterAsync(string nome, string email, string senha)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            {
                throw new Exception("Este email já está em uso.");
            }

            var passwordErrors = ValidatePasswordComplexity(senha);
            if (passwordErrors.Any())
            {
                throw new Exception(string.Join(" ", passwordErrors));
            }

            // --- [MUDANÇA] Gerando código de 6 dígitos ---
            var confirmationCode = _random.Next(100000, 999999).ToString();
            
            var newUser = new Usuario
            {
                Nome = nome,
                Email = email,
                Senha = Argon2.Hash(senha),
                TipoUsuario = "Cliente",
                EmailConfirmado = false,
                TokenConfirmacaoEmail = confirmationCode, // Salva o código
                // --- [MUDANÇA] Adicionando expiração de 10 minutos ---
                // (Requer a propriedade 'TokenConfirmacaoEmailExpiracao' no modelo Usuario)
                TokenConfirmacaoEmailExpiracao = DateTime.UtcNow.AddMinutes(10) 
            };

            await _context.Usuarios.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        // --- [MUDANÇA] Método ConfirmEmailAsync atualizado ---
        public async Task<bool> ConfirmEmailAsync(string email, string codigo)
        {
            // 1. Encontra o usuário pelo email
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // 2. Verifica se o código está correto e se não expirou
            bool codigoCorreto = user.TokenConfirmacaoEmail == codigo;
            bool naoExpirado = user.TokenConfirmacaoEmailExpiracao > DateTime.UtcNow;

            if (codigoCorreto && naoExpirado)
            {
                // 3. Sucesso: Confirma o email e limpa os dados
                user.EmailConfirmado = true;
                user.TokenConfirmacaoEmail = null;
                user.TokenConfirmacaoEmailExpiracao = null;
                
                await _context.SaveChangesAsync();
                return true;
            }

            // 4. Falha (código errado ou expirado)
            return false;
        }
        
        public async Task<Usuario?> ValidateCredentialsAsync(string email, string senha)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && Argon2.Verify(user.Senha, senha))
            {
                return user;
            }
            return null;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null || !Argon2.Verify(user.Senha, currentPassword))
            {
                return false;
            }

            var passwordErrors = ValidatePasswordComplexity(newPassword);
            if (passwordErrors.Any()) throw new Exception(string.Join(" ", passwordErrors));

            user.Senha = Argon2.Hash(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Usuario?> InitiatePasswordResetAsync(string email)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            // --- [MUDANÇA] Gerando código de 6 dígitos ---
            var code = _random.Next(100000, 999999).ToString();
            
            user.ResetPasswordToken = code;
            // --- [MUDANÇA] Reduzindo expiração para 10 minutos (mais seguro para OTP) ---
            user.ResetPasswordTokenExpires = DateTime.UtcNow.AddMinutes(10); 
            
            await _context.SaveChangesAsync();
            return user;
        }

        // --- [MUDANÇA] Método ResetPasswordAsync atualizado ---
        public async Task<bool> ResetPasswordAsync(string email, string codigo, string newPassword)
        {
            // 1. Encontra o usuário pelo email
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // 2. Verifica se o código está correto e se não expirou
            bool codigoCorreto = user.ResetPasswordToken == codigo;
            bool naoExpirado = user.ResetPasswordTokenExpires > DateTime.UtcNow;

            if (user == null || !codigoCorreto || !naoExpirado)
            {
                 return false; // Usuário não encontrado, código errado ou expirado
            }

            // 3. Valida a complexidade da nova senha
            var passwordErrors = ValidatePasswordComplexity(newPassword);
            if (passwordErrors.Any()) throw new Exception(string.Join(" ", passwordErrors));

            // 4. Sucesso: Atualiza a senha e limpa os dados
            user.Senha = Argon2.Hash(newPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpires = null;
            
            await _context.SaveChangesAsync();
            return true;
        }

        private List<string> ValidatePasswordComplexity(string password)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                errors.Add("A senha deve ter no mínimo 8 caracteres.");
            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("A senha deve ter no mínimo uma letra maiúscula.");
            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("A senha deve ter no mínimo uma letra minúscula.");
            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("A senha deve ter no mínimo um número.");
            if (!Regex.IsMatch(password, @"[\W_]")) // \W é "não-palavra", _ é para incluir underscore
                errors.Add("A senha deve ter no mínimo um caractere especial (ex: !@#$).");
            
            return errors;
        }
    }
}
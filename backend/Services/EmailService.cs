// Local: ChatbotApiNova/Services/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace ChatbotApi.Services
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string para, string assunto, string corpo);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmailAsync(string para, string assunto, string corpo)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _config["EmailSettings:SenderName"], 
                    _config["EmailSettings:SenderEmail"])
                );
                email.To.Add(MailboxAddress.Parse(para));
                email.Subject = assunto;
                email.Body = new TextPart(TextFormat.Html) { Text = corpo };

                using var smtp = new SmtpClient();
                
                // Convertendo a porta de string para int. Se der erro aqui, o catch vai pegar.
                var port = int.Parse(_config["EmailSettings:Port"]!);

                await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Se qualquer etapa dentro do 'try' falhar, o código cairá aqui.
                // Em vez de travar o programa, a mensagem de erro será exibida no console.
                // Em um projeto real, você usaria um sistema de logging aqui.
                Console.WriteLine($"ERRO AO ENVIAR E-MAIL: {ex.Message}");
                
                // Opcional: Se você ainda quiser que o erro pare a execução, remova o comentário da linha abaixo.
                // throw;
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ChatbotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _fabricaHttpClient;
        private readonly IConfiguration _configuracao;

        public ChatController(IHttpClientFactory fabricaHttpClient, IConfiguration configuracao)
        {
            _fabricaHttpClient = fabricaHttpClient;
            _configuracao = configuracao;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensagem([FromBody] MensagemUsuario requisicao)
        {
            var chaveApi = _configuracao["GeminiApiKey"];
            if (string.IsNullOrEmpty(chaveApi))
            {
                return StatusCode(500, new { resposta = "Erro: A chave da API do Gemini não foi configurada no servidor." });
            }

            var urlGemini = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={chaveApi}";

            string promptSistema = "Você é o 'Coffeebot', um assistente virtual. Sua função é responder a problemas técnicos dos clientes a cerca de sistemas e tecnologia. Responda sempre em português do Brasil e de forma clara e objetiva. Se não souber a resposta, diga para iniciar um chamado e sempre comece se apresetando pelo seu nome";

            string promptFinal = $"{promptSistema}\n\nPERGUNTA DO USUÁRIO:\n{requisicao.Mensagem}";

            var corpoRequisicaoGemini = new
            {
                contents = new[] { new { parts = new[] { new { text = promptFinal } } } }
            };

            var clienteHttp = _fabricaHttpClient.CreateClient();
            var requisicaoJson = JsonSerializer.Serialize(corpoRequisicaoGemini);
            var conteudo = new StringContent(requisicaoJson, Encoding.UTF8, "application/json");

            try
            {
                var respostaHttp = await clienteHttp.PostAsync(urlGemini, conteudo);

                // Lemos a resposta JSON do servidor do Gemini
                var respostaJson = await respostaHttp.Content.ReadAsStringAsync();

                // --- ADICIONE ESTAS DUAS LINHAS PARA "ESPIAR" A RESPOSTA ---
                Console.WriteLine("--- RESPOSTA RECEBIDA DO GEMINI ---");
                Console.WriteLine(respostaJson);
                // -----------------------------------------------------------

                if (!respostaHttp.IsSuccessStatusCode)
                {
                    var conteudoErro = await respostaHttp.Content.ReadAsStringAsync();
                    return StatusCode((int)respostaHttp.StatusCode, new { resposta = $"Erro ao chamar a API do Gemini: {conteudoErro}" });
                }
                
                using var documentoJson = JsonDocument.Parse(respostaJson);
                var texto = documentoJson.RootElement
                                         .GetProperty("candidates")[0]
                                         .GetProperty("content")
                                         .GetProperty("parts")[0]
                                         .GetProperty("text")
                                         .GetString();

                return Ok(new { resposta = texto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { resposta = $"Erro interno no servidor: {ex.Message}" });
            }
        }
    }

    public class MensagemUsuario
    {
        public string? Mensagem { get; set; }
    }
}
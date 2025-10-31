// Local: Data/DataSeeder.cs
using ChatbotApi.Data;
using ChatbotApi.Models;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatbotApi.Services
{
    public static class DataSeeder
    {
        private const string SENHA_PADRAO_TECNICO = "PadraoTecnico@123";
        private const string SENHA_PADRAO_CLIENTE = "PadraoCliente@123";

        // --- MÉTODO PRINCIPAL ATUALIZADO ---
        public static async Task SeedDatabaseAsync(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // --- 1. Semeia os Técnicos (e captura os objetos TECNICO) ---
                //    (Note que 'tecEduardo' e 'tecCaio' são agora do tipo 'Tecnico')
                await SeedTecnicoAsync(context, "tecnicogabriel", "tec1@empresa.com");
                var tecEduardo = await SeedTecnicoAsync(context, "tecnicoeduardo", "tec2@empresa.com"); // <-- Capturado para o ticket "Completo"
                await SeedTecnicoAsync(context, "tecnicodavi", "tec3@empresa.com");
                await SeedTecnicoAsync(context, "tecnicopedro", "tec4@empresa.com");
                var tecCaio = await SeedTecnicoAsync(context, "tecnicocaio", "tec5@empresa.com"); // <-- Capturado para o ticket "Em Andamento"

                // --- 2. Semeia um Cliente de Exemplo ---
                var clienteExemplo = await SeedClienteAsync(context, "Cliente Exemplo", "cliente@empresa.com");

                // --- 3. Semeia os Chamados ---
                //    (Passamos o IdUsuario do cliente e os IdTecnico dos técnicos)
                await SeedChamadosAsync(context, clienteExemplo.IdUsuario, tecCaio.IdTecnico, tecEduardo.IdTecnico); 
            }
        }

        // --- MÉTODO ATUALIZADO: Agora retorna o 'Tecnico' ---
        private static async Task<Tecnico> SeedTecnicoAsync(AppDbContext context, string nome, string email)
        {
            // --- PASSO 1: Encontra ou Cria o 'Usuario' ---
            var usuarioTecnico = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuarioTecnico == null)
            {
                usuarioTecnico = new Usuario
                {
                    Nome = nome,
                    Email = email,
                    Senha = Argon2.Hash(SENHA_PADRAO_TECNICO),
                    EmailConfirmado = true,
                    TipoUsuario = "Tecnico",
                    TokenConfirmacaoEmail = null,
                    TokenConfirmacaoEmailExpiracao = null,
                    ResetPasswordToken = null,
                    ResetPasswordTokenExpires = null
                };
                await context.Usuarios.AddAsync(usuarioTecnico);
                await context.SaveChangesAsync(); // Salva o Usuario para obter o IdUsuario
            }

            // --- PASSO 2 (NOVO): Encontra ou Cria a entrada 'Tecnico' ---
            //    Isto é o que corrige o erro da Chave Estrangeira
            var tecnico = await context.Tecnicos.FirstOrDefaultAsync(t => t.IdUsuario == usuarioTecnico.IdUsuario);
            if (tecnico == null)
            {
                tecnico = new Tecnico
                {
                    IdUsuario = usuarioTecnico.IdUsuario
                    // (Se a sua tabela 'Tecnicos' tiver outras colunas
                    //  obrigatórias, como 'Especialidade', defina-as aqui)
                };
                await context.Tecnicos.AddAsync(tecnico);
                await context.SaveChangesAsync(); // Salva o Tecnico para obter o IdTecnico
            }
            
            // --- PASSO 3: Retorna o objeto 'Tecnico' ---
            return tecnico; 
        }

        // --- Método para criar um Cliente (Sem alteração) ---
        private static async Task<Usuario> SeedClienteAsync(AppDbContext context, string nome, string email)
        {
            var cliente = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (cliente == null)
            {
                cliente = new Usuario
                {
                    Nome = nome,
                    Email = email,
                    Senha = Argon2.Hash(SENHA_PADRAO_CLIENTE),
                    EmailConfirmado = true,
                    TipoUsuario = "Cliente"
                };
                await context.Usuarios.AddAsync(cliente);
                await context.SaveChangesAsync();
            }
            return cliente; 
        }

        // --- MÉTODO ATUALIZADO: Recebe os IDs de 'Tecnico' ---
        // (idTecnicoAndamento e idTecnicoCompleto são agora os IDs da tabela 'Tecnicos')
        private static async Task SeedChamadosAsync(AppDbContext context, int idUsuarioCliente, int idTecnicoAndamento, int idTecnicoCompleto)
        {
            if (!await context.Chamados.AnyAsync())
            {
                var chamadosParaAdicionar = new List<Chamado>
                {
                    // (Os 4 chamados originais sem atribuição)
                    new Chamado {
                        Titulo = "Servidor principal offline",
                        Descricao = "O servidor principal (SRV-01) não está a responder...",
                        DataAbertura = DateTime.UtcNow.AddDays(-2),
                        Status = "Prioridade: critica",
                        Prioridade = "critica", 
                        IdUsuarioAbertura = idUsuarioCliente
                    },
                    new Chamado {
                        Titulo = "Software de vendas está lento",
                        Descricao = "O nosso software de CRM está a demorar...",
                        DataAbertura = DateTime.UtcNow.AddDays(-1),
                        Status = "Prioridade: média",
                        Prioridade = "média", 
                        IdUsuarioAbertura = idUsuarioCliente
                    },
                    new Chamado {
                        Titulo = "Mouse do financeiro quebrou",
                        Descricao = "A Carla do financeiro partiu o rato...",
                        DataAbertura = DateTime.UtcNow.AddHours(-3),
                        Status = "Prioridade: baixa",
                        Prioridade = "baixa", 
                        IdUsuarioAbertura = idUsuarioCliente
                    },
                    new Chamado {
                        Titulo = "Usuário novo não consegue conectar na VPN",
                        Descricao = "O novo funcionário, Fábio Guedes, não consegue...",
                        DataAbertura = DateTime.UtcNow.AddHours(-1),
                        Status = "A definir prioridade", 
                        Prioridade = "A definir", 
                        IdUsuarioAbertura = idUsuarioCliente
                    },

                    // (Os 2 chamados atribuídos)
                    new Chamado
                    {
                        Titulo = "Instalação de novo servidor (Projeto Alfa)",
                        Descricao = "Instalar Windows Server no novo hardware e configurar a rede.",
                        DataAbertura = DateTime.UtcNow.AddDays(-1),
                        Status = "Suporte em andamento", 
                        Prioridade = "média", 
                        IdUsuarioAbertura = idUsuarioCliente,
                        IdTecnicoResponsavel = idTecnicoAndamento // <-- ID do 'tecnicocaio' da tabela 'Tecnicos'
                    },
                    new Chamado
                    {
                        Titulo = "Troca de toner da impressora 2",
                        Descricao = "Toner da HP LaserJet do 2º andar acabou.",
                        DataAbertura = DateTime.UtcNow.AddDays(-3),
                        Status = "Suporte completo", 
                        Prioridade = "baixa", 
                        IdUsuarioAbertura = idUsuarioCliente,
                        IdTecnicoResponsavel = idTecnicoCompleto // <-- ID do 'tecnicoeduardo' da tabela 'Tecnicos'
                    }
                };

                await context.Chamados.AddRangeAsync(chamadosParaAdicionar);
                await context.SaveChangesAsync();
            }
        }
    }
}
// Usings necessários
using Microsoft.EntityFrameworkCore;
using ChatbotApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ChatbotApi.Services; 
using System.Security.Claims; 

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configuração dos Serviços ---

// Adiciona o serviço do Banco de Dados (Isto está correto)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// --- REMOVIDO: Configuração do ASP.NET Identity ---
// ...

// Adiciona os serviços padrão
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Registra todos os seus serviços customizados (Isto está correto)
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IChamadoService, ChamadoService>(); 
builder.Services.AddScoped<IContaService, ContaService>(); // <-- O seu serviço com Argon2
builder.Services.AddScoped<ITokenService, TokenService>();

// Adiciona e configura o Swagger (Correto)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona e configura a política de CORS (Correto)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAppVue", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://andersondefreitas.github.io"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Adiciona a configuração de autenticação JWT (Correto)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("A chave JWT (Jwt:Key) não está configurada no appsettings.json");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// --- Política de Autorização Customizada (Correto) ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApenasTecnicos", policy => 
        policy.RequireClaim(ClaimTypes.Role, "Tecnico"));
        
    options.AddPolicy("AdminOuTecnico", policy => 
        policy.RequireClaim(ClaimTypes.Role, "Tecnico", "Admin"));
});


// --- 2. Construção da Aplicação ---
var app = builder.Build();

// --- ADICIONADO: Chamada ao Data Seeder (Compatível com Argon2) ---
// Esta linha chama o 'DataSeeder.cs' que usa o seu AppDbContext
// e Argon2.Hash para criar os 4 técnicos.
await DataSeeder.SeedDatabaseAsync(app);


// --- 3. Configuração do Pipeline de Requisições ---
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("PermitirAppVue");

app.UseAuthentication(); // 1º "Quem é você?" (Lê o token)
app.UseAuthorization();  // 2º "O que você pode fazer?" (Aplica as políticas)

app.MapControllers();
app.Run();
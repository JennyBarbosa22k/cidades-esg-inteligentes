using System.Text;
using EnergiaSustentavel.API.Data;
using EnergiaSustentavel.API.Middleware;
using EnergiaSustentavel.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Npgsql: aceita DateTime de qualquer Kind (comportamento "legado" de timestamp).
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------------
// 1) Banco de dados (EF Core + PostgreSQL)
// ----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=energia;Username=energia;Password=energia";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ----------------------------------------------------------------------------
// 2) Injeção de dependência dos serviços (camada de negócio)
// ----------------------------------------------------------------------------
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEquipamentoService, EquipamentoService>();
builder.Services.AddScoped<ILeituraService, LeituraService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IConsumoService, ConsumoService>();

// ----------------------------------------------------------------------------
// 3) Autenticação e autorização (JWT Bearer)
// ----------------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? "CHAVE_SECRETA_PADRAO_TROQUE_EM_PRODUCAO_1234567890_ABCDEFGHIJ";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ----------------------------------------------------------------------------
// 4) Controllers + Swagger (com suporte a JWT)
// ----------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Eficiência Energética (ESG)",
        Version = "v1",
        Description = "API RESTful para monitoramento de consumo de energia, alertas automáticos e relatórios (Tema ESG: Eficiência Energética e Sustentabilidade)."
    });

    // Botão "Authorize" no Swagger para enviar o token JWT.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT. Exemplo: cole apenas o token (sem a palavra 'Bearer')."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ----------------------------------------------------------------------------
// 5) Cria as tabelas (se necessário) e popula dados iniciais (apenas em banco relacional)
// ----------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational())
    {
        DatabaseBootstrapper.EnsureTables(db, builder.Configuration["Database:Schema"]);
    }
    DbInitializer.Seed(db);
}

// ----------------------------------------------------------------------------
// 6) Pipeline HTTP
// ----------------------------------------------------------------------------
// Tratamento global de exceções (deve vir cedo no pipeline).
app.UseMiddleware<ExceptionMiddleware>();

// Swagger habilitado em todos os ambientes (staging e produção incluídos) para facilitar a demonstração.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Necessário para os testes de integração com WebApplicationFactory<Program>.
public partial class Program { }

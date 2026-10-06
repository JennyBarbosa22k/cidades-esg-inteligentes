using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EnergiaSustentavel.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace EnergiaSustentavel.API.Services;

/// <summary>
/// Implementação do serviço de token JWT. Lê as configurações da seção "Jwt"
/// do appsettings (chave, emissor, audiência e tempo de expiração).
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiraEm) GerarToken(Usuario usuario)
    {
        var chave = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:Key' ausente.");
        var emissor = _configuration["Jwt:Issuer"];
        var audiencia = _configuration["Jwt:Audience"];
        var horas = int.TryParse(_configuration["Jwt:ExpiraEmHoras"], out var h) ? h : 8;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Perfil),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
        var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiraEm = DateTime.UtcNow.AddHours(horas);

        var token = new JwtSecurityToken(
            issuer: emissor,
            audience: audiencia,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}

using EnergiaSustentavel.API.Data;
using EnergiaSustentavel.API.Services;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnergiaSustentavel.API.Controllers;

/// <summary>
/// Endpoints de autenticação. Responsável por validar credenciais e emitir o
/// token JWT usado para acessar os endpoints protegidos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    /// <summary>Realiza o login e retorna um token JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenViewModel>> Login([FromBody] LoginViewModel model)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        // Verificação de senha com BCrypt.
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(model.Senha, usuario.SenhaHash))
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        var (token, expiraEm) = _tokenService.GerarToken(usuario);

        return Ok(new TokenViewModel
        {
            Token = token,
            ExpiraEm = expiraEm,
            Tipo = "Bearer",
            Perfil = usuario.Perfil
        });
    }
}

using EnergiaSustentavel.API.Models;

namespace EnergiaSustentavel.API.Services;

public interface ITokenService
{
    /// <summary>Gera um token JWT assinado para o usuário informado.</summary>
    (string token, DateTime expiraEm) GerarToken(Usuario usuario);
}

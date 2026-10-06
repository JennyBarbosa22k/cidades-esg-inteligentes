namespace EnergiaSustentavel.API.Models;

/// <summary>
/// Representa um usuário do sistema. Usado apenas para autenticação/autorização
/// (login e emissão de token JWT). O foco do projeto NÃO é o CRUD de usuários.
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>Hash da senha gerado com BCrypt. Nunca armazenamos a senha em texto puro.</summary>
    public string SenhaHash { get; set; } = string.Empty;

    /// <summary>Perfil de acesso: "Admin" ou "Operador".</summary>
    public string Perfil { get; set; } = "Operador";
}

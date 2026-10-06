using System.ComponentModel.DataAnnotations;

namespace EnergiaSustentavel.API.ViewModels;

/// <summary>Dados enviados pelo cliente para realizar login.</summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter ao menos 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

/// <summary>Resposta retornada após login bem-sucedido.</summary>
public class TokenViewModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public string Tipo { get; set; } = "Bearer";
    public string Perfil { get; set; } = string.Empty;
}

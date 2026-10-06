using System.ComponentModel.DataAnnotations;

namespace EnergiaSustentavel.API.ViewModels;

/// <summary>Dados de entrada para criar/atualizar um equipamento (com validação).</summary>
public class EquipamentoCreateViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A localização é obrigatória.")]
    [StringLength(150, ErrorMessage = "A localização deve ter no máximo 150 caracteres.")]
    public string Localizacao { get; set; } = string.Empty;

    [Range(0.01, 1_000_000, ErrorMessage = "A potência deve ser maior que zero.")]
    public decimal PotenciaWatts { get; set; }

    [Range(0.01, 1_000_000, ErrorMessage = "O limite de consumo deve ser maior que zero.")]
    public decimal LimiteConsumoKwh { get; set; }

    public bool Ativo { get; set; } = true;
}

/// <summary>Dados de saída de um equipamento.</summary>
public class EquipamentoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public decimal PotenciaWatts { get; set; }
    public decimal LimiteConsumoKwh { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}

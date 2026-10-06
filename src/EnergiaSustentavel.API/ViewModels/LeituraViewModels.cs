using System.ComponentModel.DataAnnotations;

namespace EnergiaSustentavel.API.ViewModels;

/// <summary>Dados de entrada para registrar uma nova leitura de consumo.</summary>
public class LeituraCreateViewModel
{
    [Required(ErrorMessage = "O equipamento é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Informe um equipamento válido.")]
    public int EquipamentoId { get; set; }

    [Range(0.0, 1_000_000, ErrorMessage = "O consumo deve ser maior ou igual a zero.")]
    public decimal ConsumoKwh { get; set; }

    /// <summary>Opcional. Se não informado, usa a data/hora atual (UTC).</summary>
    public DateTime? DataLeitura { get; set; }

    [StringLength(50, ErrorMessage = "A fonte deve ter no máximo 50 caracteres.")]
    public string Fonte { get; set; } = "Sensor IoT";
}

/// <summary>Dados de saída de uma leitura de consumo.</summary>
public class LeituraViewModel
{
    public int Id { get; set; }
    public int EquipamentoId { get; set; }
    public string EquipamentoNome { get; set; } = string.Empty;
    public decimal ConsumoKwh { get; set; }
    public DateTime DataLeitura { get; set; }
    public string Fonte { get; set; } = string.Empty;

    /// <summary>Indica se esta leitura gerou um alerta (ultrapassou o limite).</summary>
    public bool GerouAlerta { get; set; }
}

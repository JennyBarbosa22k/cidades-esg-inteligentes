namespace EnergiaSustentavel.API.Models;

/// <summary>
/// Alerta gerado automaticamente quando o consumo de uma leitura ultrapassa
/// o limite configurado para o equipamento.
/// </summary>
public class Alerta
{
    public int Id { get; set; }

    public int EquipamentoId { get; set; }

    public string Mensagem { get; set; } = string.Empty;

    /// <summary>Consumo (kWh) que disparou o alerta.</summary>
    public decimal ConsumoRegistrado { get; set; }

    /// <summary>Limite (kWh) que foi ultrapassado.</summary>
    public decimal LimiteUltrapassado { get; set; }

    /// <summary>Severidade calculada: "Baixa", "Media" ou "Alta".</summary>
    public string Severidade { get; set; } = "Media";

    public bool Resolvido { get; set; } = false;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvidoEm { get; set; }

    // Navegação
    public Equipamento? Equipamento { get; set; }
}

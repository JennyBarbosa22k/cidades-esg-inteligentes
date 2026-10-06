namespace EnergiaSustentavel.API.Models;

/// <summary>
/// Equipamento monitorado (ex.: ar-condicionado, servidor, máquina industrial).
/// Cada equipamento possui um limite de consumo (kWh) que, ao ser ultrapassado,
/// dispara automaticamente um alerta.
/// </summary>
public class Equipamento
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Localizacao { get; set; } = string.Empty;

    /// <summary>Potência nominal do equipamento em watts.</summary>
    public decimal PotenciaWatts { get; set; }

    /// <summary>Limite de consumo (kWh) por leitura. Acima disso, gera alerta.</summary>
    public decimal LimiteConsumoKwh { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Relacionamentos (navegação)
    public ICollection<LeituraConsumo> Leituras { get; set; } = new List<LeituraConsumo>();
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}

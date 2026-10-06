namespace EnergiaSustentavel.API.Models;

/// <summary>
/// Leitura de consumo de energia coletada de um equipamento (tipicamente enviada
/// por um sensor IoT). É o dado central do sistema de monitoramento.
/// </summary>
public class LeituraConsumo
{
    public int Id { get; set; }

    public int EquipamentoId { get; set; }

    /// <summary>Consumo registrado nesta leitura, em kWh.</summary>
    public decimal ConsumoKwh { get; set; }

    public DateTime DataLeitura { get; set; } = DateTime.UtcNow;

    /// <summary>Origem da leitura (ex.: "Sensor IoT", "Medidor Manual").</summary>
    public string Fonte { get; set; } = "Sensor IoT";

    // Navegação
    public Equipamento? Equipamento { get; set; }
}

namespace EnergiaSustentavel.API.ViewModels;

/// <summary>Dados de saída de um alerta.</summary>
public class AlertaViewModel
{
    public int Id { get; set; }
    public int EquipamentoId { get; set; }
    public string EquipamentoNome { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public decimal ConsumoRegistrado { get; set; }
    public decimal LimiteUltrapassado { get; set; }
    public string Severidade { get; set; } = string.Empty;
    public bool Resolvido { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? ResolvidoEm { get; set; }
}

/// <summary>Resumo agregado de consumo por equipamento (consulta analítica).</summary>
public class ConsumoResumoViewModel
{
    public int EquipamentoId { get; set; }
    public string EquipamentoNome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public decimal ConsumoTotalKwh { get; set; }
    public decimal ConsumoMedioKwh { get; set; }
    public decimal MaiorLeituraKwh { get; set; }
    public int QuantidadeLeituras { get; set; }
    public int TotalAlertas { get; set; }
    public decimal LimiteConfigurado { get; set; }
}

/// <summary>Indicadores gerais do painel (dashboard).</summary>
public class ConsumoDashboardViewModel
{
    public int TotalEquipamentos { get; set; }
    public int EquipamentosAtivos { get; set; }
    public decimal ConsumoTotalKwh { get; set; }
    public int TotalLeituras { get; set; }
    public int AlertasAbertos { get; set; }
    public int AlertasResolvidos { get; set; }
    public IEnumerable<ConsumoResumoViewModel> TopConsumidores { get; set; } = new List<ConsumoResumoViewModel>();
}

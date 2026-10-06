using EnergiaSustentavel.API.ViewModels;

namespace EnergiaSustentavel.API.Services;

public interface IConsumoService
{
    /// <summary>Resumo agregado de consumo por equipamento.</summary>
    Task<IEnumerable<ConsumoResumoViewModel>> ResumoPorEquipamentoAsync();

    /// <summary>Resumo de consumo de um equipamento específico.</summary>
    Task<ConsumoResumoViewModel> ResumoDoEquipamentoAsync(int equipamentoId);

    /// <summary>Indicadores gerais do painel.</summary>
    Task<ConsumoDashboardViewModel> ObterDashboardAsync();
}

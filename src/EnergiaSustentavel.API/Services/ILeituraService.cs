using EnergiaSustentavel.API.ViewModels;

namespace EnergiaSustentavel.API.Services;

public interface ILeituraService
{
    Task<ResultadoPaginado<LeituraViewModel>> ListarAsync(ParametrosPaginacao parametros, int? equipamentoId);
    Task<LeituraViewModel> ObterPorIdAsync(int id);

    /// <summary>
    /// Registra uma nova leitura. Se o consumo ultrapassar o limite do equipamento,
    /// gera automaticamente um alerta com severidade proporcional ao excesso.
    /// </summary>
    Task<LeituraViewModel> RegistrarAsync(LeituraCreateViewModel model);
}

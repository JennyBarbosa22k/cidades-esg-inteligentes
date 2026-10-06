using EnergiaSustentavel.API.ViewModels;

namespace EnergiaSustentavel.API.Services;

public interface IEquipamentoService
{
    Task<ResultadoPaginado<EquipamentoViewModel>> ListarAsync(ParametrosPaginacao parametros);
    Task<EquipamentoViewModel> ObterPorIdAsync(int id);
    Task<EquipamentoViewModel> CriarAsync(EquipamentoCreateViewModel model);
    Task<EquipamentoViewModel> AtualizarAsync(int id, EquipamentoCreateViewModel model);
    Task RemoverAsync(int id);
}

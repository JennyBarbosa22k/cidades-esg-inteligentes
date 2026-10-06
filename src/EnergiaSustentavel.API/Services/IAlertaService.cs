using EnergiaSustentavel.API.ViewModels;

namespace EnergiaSustentavel.API.Services;

public interface IAlertaService
{
    Task<ResultadoPaginado<AlertaViewModel>> ListarAsync(ParametrosPaginacao parametros, bool? resolvido);
    Task<AlertaViewModel> ResolverAsync(int id);
}

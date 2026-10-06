using EnergiaSustentavel.API.Services;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnergiaSustentavel.API.Controllers;

/// <summary>
/// Gerencia os alertas de consumo. A listagem é pública (com paginação e filtro
/// por status); resolver um alerta é um endpoint crítico e exige autenticação.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _service;

    public AlertasController(IAlertaService service) => _service = service;

    /// <summary>Lista alertas de forma paginada, com filtro opcional por status (resolvido).</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultadoPaginado<AlertaViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<AlertaViewModel>>> Listar(
        [FromQuery] ParametrosPaginacao parametros,
        [FromQuery] bool? resolvido)
    {
        var resultado = await _service.ListarAsync(parametros, resolvido);
        return Ok(resultado);
    }

    /// <summary>Marca um alerta como resolvido (requer autenticação).</summary>
    [HttpPut("{id:int}/resolver")]
    [Authorize]
    [ProducesResponseType(typeof(AlertaViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaViewModel>> Resolver(int id)
    {
        var alerta = await _service.ResolverAsync(id);
        return Ok(alerta);
    }
}

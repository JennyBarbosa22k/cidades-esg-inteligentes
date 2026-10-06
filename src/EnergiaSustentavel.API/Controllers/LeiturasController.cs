using EnergiaSustentavel.API.Services;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnergiaSustentavel.API.Controllers;

/// <summary>
/// Gerencia as leituras de consumo de energia. O registro de leitura é um endpoint
/// crítico (protegido por JWT) e dispara automaticamente alertas quando o consumo
/// ultrapassa o limite do equipamento.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeiturasController : ControllerBase
{
    private readonly ILeituraService _service;

    public LeiturasController(ILeituraService service) => _service = service;

    /// <summary>Lista leituras de forma paginada, com filtro opcional por equipamento.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultadoPaginado<LeituraViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<LeituraViewModel>>> Listar(
        [FromQuery] ParametrosPaginacao parametros,
        [FromQuery] int? equipamentoId)
    {
        var resultado = await _service.ListarAsync(parametros, equipamentoId);
        return Ok(resultado);
    }

    /// <summary>Obtém uma leitura pelo ID.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LeituraViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraViewModel>> ObterPorId(int id)
    {
        var leitura = await _service.ObterPorIdAsync(id);
        return Ok(leitura);
    }

    /// <summary>
    /// Registra uma nova leitura de consumo (requer autenticação).
    /// Gera alerta automaticamente se o consumo exceder o limite do equipamento.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(LeituraViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraViewModel>> Registrar([FromBody] LeituraCreateViewModel model)
    {
        var leitura = await _service.RegistrarAsync(model);
        return CreatedAtAction(nameof(ObterPorId), new { id = leitura.Id }, leitura);
    }
}

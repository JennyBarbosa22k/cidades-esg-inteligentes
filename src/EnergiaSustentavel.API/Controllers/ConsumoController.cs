using EnergiaSustentavel.API.Services;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnergiaSustentavel.API.Controllers;

/// <summary>
/// Endpoints analíticos de consumo de energia (relatórios agregados e painel).
/// Demonstra consultas otimizadas com agregação executada no banco de dados.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConsumoController : ControllerBase
{
    private readonly IConsumoService _service;

    public ConsumoController(IConsumoService service) => _service = service;

    /// <summary>Indicadores gerais do painel (totais e top consumidores).</summary>
    [HttpGet("dashboard")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ConsumoDashboardViewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<ConsumoDashboardViewModel>> Dashboard()
    {
        var dashboard = await _service.ObterDashboardAsync();
        return Ok(dashboard);
    }

    /// <summary>Resumo agregado de consumo por equipamento.</summary>
    [HttpGet("resumo")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ConsumoResumoViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ConsumoResumoViewModel>>> Resumo()
    {
        var resumos = await _service.ResumoPorEquipamentoAsync();
        return Ok(resumos);
    }

    /// <summary>Resumo de consumo de um equipamento específico (requer autenticação).</summary>
    [HttpGet("equipamento/{equipamentoId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ConsumoResumoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsumoResumoViewModel>> PorEquipamento(int equipamentoId)
    {
        var resumo = await _service.ResumoDoEquipamentoAsync(equipamentoId);
        return Ok(resumo);
    }
}

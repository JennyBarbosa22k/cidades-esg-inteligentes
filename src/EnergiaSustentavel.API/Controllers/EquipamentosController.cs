using EnergiaSustentavel.API.Services;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnergiaSustentavel.API.Controllers;

/// <summary>
/// Gerencia os equipamentos monitorados.
/// Listagem e consulta são públicas; criação, atualização e exclusão exigem token JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EquipamentosController : ControllerBase
{
    private readonly IEquipamentoService _service;

    public EquipamentosController(IEquipamentoService service) => _service = service;

    /// <summary>Lista equipamentos de forma paginada.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultadoPaginado<EquipamentoViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginado<EquipamentoViewModel>>> Listar(
        [FromQuery] ParametrosPaginacao parametros)
    {
        var resultado = await _service.ListarAsync(parametros);
        return Ok(resultado);
    }

    /// <summary>Obtém um equipamento pelo ID.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EquipamentoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipamentoViewModel>> ObterPorId(int id)
    {
        var equipamento = await _service.ObterPorIdAsync(id);
        return Ok(equipamento);
    }

    /// <summary>Cria um novo equipamento (requer autenticação).</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EquipamentoViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EquipamentoViewModel>> Criar([FromBody] EquipamentoCreateViewModel model)
    {
        var criado = await _service.CriarAsync(model);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    /// <summary>Atualiza um equipamento existente (requer autenticação).</summary>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(EquipamentoViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipamentoViewModel>> Atualizar(int id, [FromBody] EquipamentoCreateViewModel model)
    {
        var atualizado = await _service.AtualizarAsync(id, model);
        return Ok(atualizado);
    }

    /// <summary>Remove um equipamento (requer perfil Admin).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await _service.RemoverAsync(id);
        return NoContent();
    }
}

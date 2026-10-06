using EnergiaSustentavel.API.Data;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EnergiaSustentavel.API.Services;

public class AlertaService : IAlertaService
{
    private readonly AppDbContext _db;

    public AlertaService(AppDbContext db) => _db = db;

    public async Task<ResultadoPaginado<AlertaViewModel>> ListarAsync(ParametrosPaginacao p, bool? resolvido)
    {
        var query = _db.Alertas.AsNoTracking().AsQueryable();

        // Filtro opcional por status (usa o índice em Resolvido).
        if (resolvido.HasValue)
            query = query.Where(a => a.Resolvido == resolvido.Value);

        query = query.OrderByDescending(a => a.CriadoEm);

        var total = await query.CountAsync();

        var itens = await query
            .Skip((p.Pagina - 1) * p.TamanhoPagina)
            .Take(p.TamanhoPagina)
            .Select(a => new AlertaViewModel
            {
                Id = a.Id,
                EquipamentoId = a.EquipamentoId,
                EquipamentoNome = a.Equipamento!.Nome,
                Mensagem = a.Mensagem,
                ConsumoRegistrado = a.ConsumoRegistrado,
                LimiteUltrapassado = a.LimiteUltrapassado,
                Severidade = a.Severidade,
                Resolvido = a.Resolvido,
                CriadoEm = a.CriadoEm,
                ResolvidoEm = a.ResolvidoEm
            })
            .ToListAsync();

        return new ResultadoPaginado<AlertaViewModel>(itens, total, p.Pagina, p.TamanhoPagina);
    }

    public async Task<AlertaViewModel> ResolverAsync(int id)
    {
        var alerta = await _db.Alertas.Include(a => a.Equipamento).FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new RecursoNaoEncontradoException($"Alerta {id} não encontrado.");

        if (alerta.Resolvido)
            throw new RegraNegocioException("Este alerta já está resolvido.");

        alerta.Resolvido = true;
        alerta.ResolvidoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new AlertaViewModel
        {
            Id = alerta.Id,
            EquipamentoId = alerta.EquipamentoId,
            EquipamentoNome = alerta.Equipamento!.Nome,
            Mensagem = alerta.Mensagem,
            ConsumoRegistrado = alerta.ConsumoRegistrado,
            LimiteUltrapassado = alerta.LimiteUltrapassado,
            Severidade = alerta.Severidade,
            Resolvido = alerta.Resolvido,
            CriadoEm = alerta.CriadoEm,
            ResolvidoEm = alerta.ResolvidoEm
        };
    }
}

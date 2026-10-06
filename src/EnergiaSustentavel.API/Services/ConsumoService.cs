using EnergiaSustentavel.API.Data;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EnergiaSustentavel.API.Services;

public class ConsumoService : IConsumoService
{
    private readonly AppDbContext _db;

    public ConsumoService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ConsumoResumoViewModel>> ResumoPorEquipamentoAsync()
    {
        // Consulta analítica: agrega leituras por equipamento (Sum/Avg/Max/Count)
        // diretamente no banco, evitando trazer todas as linhas para a memória.
        var resumos = await _db.Equipamentos
            .AsNoTracking()
            .Select(e => new ConsumoResumoViewModel
            {
                EquipamentoId = e.Id,
                EquipamentoNome = e.Nome,
                Localizacao = e.Localizacao,
                LimiteConfigurado = e.LimiteConsumoKwh,
                QuantidadeLeituras = e.Leituras.Count,
                ConsumoTotalKwh = e.Leituras.Sum(l => (decimal?)l.ConsumoKwh) ?? 0,
                ConsumoMedioKwh = e.Leituras.Any() ? e.Leituras.Average(l => l.ConsumoKwh) : 0,
                MaiorLeituraKwh = e.Leituras.Any() ? e.Leituras.Max(l => l.ConsumoKwh) : 0,
                TotalAlertas = e.Alertas.Count
            })
            .OrderByDescending(r => r.ConsumoTotalKwh)
            .ToListAsync();

        return resumos;
    }

    public async Task<ConsumoResumoViewModel> ResumoDoEquipamentoAsync(int equipamentoId)
    {
        var resumo = await _db.Equipamentos
            .AsNoTracking()
            .Where(e => e.Id == equipamentoId)
            .Select(e => new ConsumoResumoViewModel
            {
                EquipamentoId = e.Id,
                EquipamentoNome = e.Nome,
                Localizacao = e.Localizacao,
                LimiteConfigurado = e.LimiteConsumoKwh,
                QuantidadeLeituras = e.Leituras.Count,
                ConsumoTotalKwh = e.Leituras.Sum(l => (decimal?)l.ConsumoKwh) ?? 0,
                ConsumoMedioKwh = e.Leituras.Any() ? e.Leituras.Average(l => l.ConsumoKwh) : 0,
                MaiorLeituraKwh = e.Leituras.Any() ? e.Leituras.Max(l => l.ConsumoKwh) : 0,
                TotalAlertas = e.Alertas.Count
            })
            .FirstOrDefaultAsync()
            ?? throw new RecursoNaoEncontradoException($"Equipamento {equipamentoId} não encontrado.");

        return resumo;
    }

    public async Task<ConsumoDashboardViewModel> ObterDashboardAsync()
    {
        var totalEquip = await _db.Equipamentos.CountAsync();
        var ativos = await _db.Equipamentos.CountAsync(e => e.Ativo);
        var totalLeituras = await _db.Leituras.CountAsync();
        var consumoTotal = await _db.Leituras.SumAsync(l => (decimal?)l.ConsumoKwh) ?? 0;
        var alertasAbertos = await _db.Alertas.CountAsync(a => !a.Resolvido);
        var alertasResolvidos = await _db.Alertas.CountAsync(a => a.Resolvido);

        var top = (await ResumoPorEquipamentoAsync()).Take(5);

        return new ConsumoDashboardViewModel
        {
            TotalEquipamentos = totalEquip,
            EquipamentosAtivos = ativos,
            ConsumoTotalKwh = consumoTotal,
            TotalLeituras = totalLeituras,
            AlertasAbertos = alertasAbertos,
            AlertasResolvidos = alertasResolvidos,
            TopConsumidores = top
        };
    }
}

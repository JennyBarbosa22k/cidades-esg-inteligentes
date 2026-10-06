using EnergiaSustentavel.API.Data;
using EnergiaSustentavel.API.Models;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EnergiaSustentavel.API.Services;

public class LeituraService : ILeituraService
{
    private readonly AppDbContext _db;

    public LeituraService(AppDbContext db) => _db = db;

    public async Task<ResultadoPaginado<LeituraViewModel>> ListarAsync(ParametrosPaginacao p, int? equipamentoId)
    {
        var query = _db.Leituras.AsNoTracking().AsQueryable();

        // Filtro opcional por equipamento (usa o índice composto definido no DbContext).
        if (equipamentoId.HasValue)
            query = query.Where(l => l.EquipamentoId == equipamentoId.Value);

        query = query.OrderByDescending(l => l.DataLeitura);

        var total = await query.CountAsync();

        var itens = await query
            .Skip((p.Pagina - 1) * p.TamanhoPagina)
            .Take(p.TamanhoPagina)
            .Select(l => new LeituraViewModel
            {
                Id = l.Id,
                EquipamentoId = l.EquipamentoId,
                EquipamentoNome = l.Equipamento!.Nome,
                ConsumoKwh = l.ConsumoKwh,
                DataLeitura = l.DataLeitura,
                Fonte = l.Fonte,
                GerouAlerta = l.ConsumoKwh > l.Equipamento.LimiteConsumoKwh
            })
            .ToListAsync();

        return new ResultadoPaginado<LeituraViewModel>(itens, total, p.Pagina, p.TamanhoPagina);
    }

    public async Task<LeituraViewModel> ObterPorIdAsync(int id)
    {
        var leitura = await _db.Leituras
            .AsNoTracking()
            .Include(l => l.Equipamento)
            .FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new RecursoNaoEncontradoException($"Leitura {id} não encontrada.");

        return new LeituraViewModel
        {
            Id = leitura.Id,
            EquipamentoId = leitura.EquipamentoId,
            EquipamentoNome = leitura.Equipamento!.Nome,
            ConsumoKwh = leitura.ConsumoKwh,
            DataLeitura = leitura.DataLeitura,
            Fonte = leitura.Fonte,
            GerouAlerta = leitura.ConsumoKwh > leitura.Equipamento.LimiteConsumoKwh
        };
    }

    public async Task<LeituraViewModel> RegistrarAsync(LeituraCreateViewModel model)
    {
        var equipamento = await _db.Equipamentos.FirstOrDefaultAsync(e => e.Id == model.EquipamentoId)
            ?? throw new RecursoNaoEncontradoException($"Equipamento {model.EquipamentoId} não encontrado.");

        if (!equipamento.Ativo)
            throw new RegraNegocioException("Não é possível registrar leitura para um equipamento inativo.");

        var leitura = new LeituraConsumo
        {
            EquipamentoId = equipamento.Id,
            ConsumoKwh = model.ConsumoKwh,
            DataLeitura = model.DataLeitura ?? DateTime.UtcNow,
            Fonte = string.IsNullOrWhiteSpace(model.Fonte) ? "Sensor IoT" : model.Fonte
        };

        _db.Leituras.Add(leitura);

        // ----- Regra de negócio: geração automática de alerta -----
        bool gerouAlerta = false;
        if (leitura.ConsumoKwh > equipamento.LimiteConsumoKwh)
        {
            var excesso = leitura.ConsumoKwh - equipamento.LimiteConsumoKwh;
            var percentual = equipamento.LimiteConsumoKwh > 0
                ? (excesso / equipamento.LimiteConsumoKwh) * 100
                : 100;

            var severidade = percentual switch
            {
                <= 10 => "Baixa",
                <= 30 => "Media",
                _ => "Alta"
            };

            _db.Alertas.Add(new Alerta
            {
                EquipamentoId = equipamento.Id,
                Mensagem = $"Consumo de {leitura.ConsumoKwh:N2} kWh ultrapassou o limite de " +
                           $"{equipamento.LimiteConsumoKwh:N2} kWh ({percentual:N0}% acima).",
                ConsumoRegistrado = leitura.ConsumoKwh,
                LimiteUltrapassado = equipamento.LimiteConsumoKwh,
                Severidade = severidade,
                Resolvido = false,
                CriadoEm = DateTime.UtcNow
            });
            gerouAlerta = true;
        }

        await _db.SaveChangesAsync();

        return new LeituraViewModel
        {
            Id = leitura.Id,
            EquipamentoId = leitura.EquipamentoId,
            EquipamentoNome = equipamento.Nome,
            ConsumoKwh = leitura.ConsumoKwh,
            DataLeitura = leitura.DataLeitura,
            Fonte = leitura.Fonte,
            GerouAlerta = gerouAlerta
        };
    }
}

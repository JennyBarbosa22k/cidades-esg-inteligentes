using EnergiaSustentavel.API.Data;
using EnergiaSustentavel.API.Models;
using EnergiaSustentavel.API.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EnergiaSustentavel.API.Services;

public class EquipamentoService : IEquipamentoService
{
    private readonly AppDbContext _db;

    public EquipamentoService(AppDbContext db) => _db = db;

    public async Task<ResultadoPaginado<EquipamentoViewModel>> ListarAsync(ParametrosPaginacao p)
    {
        // Otimização: AsNoTracking (somente leitura) + paginação no banco (Skip/Take).
        var query = _db.Equipamentos.AsNoTracking().OrderBy(e => e.Id);

        var total = await query.CountAsync();

        var itens = await query
            .Skip((p.Pagina - 1) * p.TamanhoPagina)
            .Take(p.TamanhoPagina)
            .Select(e => new EquipamentoViewModel
            {
                Id = e.Id,
                Nome = e.Nome,
                Localizacao = e.Localizacao,
                PotenciaWatts = e.PotenciaWatts,
                LimiteConsumoKwh = e.LimiteConsumoKwh,
                Ativo = e.Ativo,
                CriadoEm = e.CriadoEm
            })
            .ToListAsync();

        return new ResultadoPaginado<EquipamentoViewModel>(itens, total, p.Pagina, p.TamanhoPagina);
    }

    public async Task<EquipamentoViewModel> ObterPorIdAsync(int id)
    {
        var e = await _db.Equipamentos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new RecursoNaoEncontradoException($"Equipamento {id} não encontrado.");

        return Mapear(e);
    }

    public async Task<EquipamentoViewModel> CriarAsync(EquipamentoCreateViewModel model)
    {
        var entidade = new Equipamento
        {
            Nome = model.Nome,
            Localizacao = model.Localizacao,
            PotenciaWatts = model.PotenciaWatts,
            LimiteConsumoKwh = model.LimiteConsumoKwh,
            Ativo = model.Ativo,
            CriadoEm = DateTime.UtcNow
        };

        _db.Equipamentos.Add(entidade);
        await _db.SaveChangesAsync();
        return Mapear(entidade);
    }

    public async Task<EquipamentoViewModel> AtualizarAsync(int id, EquipamentoCreateViewModel model)
    {
        var entidade = await _db.Equipamentos.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new RecursoNaoEncontradoException($"Equipamento {id} não encontrado.");

        entidade.Nome = model.Nome;
        entidade.Localizacao = model.Localizacao;
        entidade.PotenciaWatts = model.PotenciaWatts;
        entidade.LimiteConsumoKwh = model.LimiteConsumoKwh;
        entidade.Ativo = model.Ativo;

        await _db.SaveChangesAsync();
        return Mapear(entidade);
    }

    public async Task RemoverAsync(int id)
    {
        var entidade = await _db.Equipamentos.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new RecursoNaoEncontradoException($"Equipamento {id} não encontrado.");

        _db.Equipamentos.Remove(entidade);
        await _db.SaveChangesAsync();
    }

    private static EquipamentoViewModel Mapear(Equipamento e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        Localizacao = e.Localizacao,
        PotenciaWatts = e.PotenciaWatts,
        LimiteConsumoKwh = e.LimiteConsumoKwh,
        Ativo = e.Ativo,
        CriadoEm = e.CriadoEm
    };
}

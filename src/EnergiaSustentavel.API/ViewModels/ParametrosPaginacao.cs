using System.ComponentModel.DataAnnotations;

namespace EnergiaSustentavel.API.ViewModels;

/// <summary>
/// Parâmetros de paginação recebidos via query string (?pagina=1&amp;tamanhoPagina=10).
/// Possui limites de segurança para evitar consultas que retornem volumes excessivos.
/// </summary>
public class ParametrosPaginacao
{
    private const int TamanhoMaximo = 100;
    private int _tamanhoPagina = 10;
    private int _pagina = 1;

    /// <summary>Número da página (começa em 1).</summary>
    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    /// <summary>Quantidade de itens por página (máximo 100).</summary>
    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value < 1 ? 10 : (value > TamanhoMaximo ? TamanhoMaximo : value);
    }
}

namespace EnergiaSustentavel.API.ViewModels;

/// <summary>
/// Envelope genérico para respostas paginadas. Inclui metadados que permitem ao
/// cliente navegar entre as páginas sem precisar carregar tudo de uma vez.
/// </summary>
public class ResultadoPaginado<T>
{
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalPaginas { get; set; }
    public bool TemPaginaAnterior => Pagina > 1;
    public bool TemProximaPagina => Pagina < TotalPaginas;
    public IEnumerable<T> Itens { get; set; } = new List<T>();

    public ResultadoPaginado() { }

    public ResultadoPaginado(IEnumerable<T> itens, int totalRegistros, int pagina, int tamanhoPagina)
    {
        Itens = itens;
        TotalRegistros = totalRegistros;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
        TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanhoPagina);
    }
}

namespace EnergiaSustentavel.API.Services;

/// <summary>Lançada quando um recurso solicitado não é encontrado (resulta em HTTP 404).</summary>
public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem) { }
}

/// <summary>Lançada para regras de negócio inválidas (resulta em HTTP 400).</summary>
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem) { }
}

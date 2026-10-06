using System.Net;
using Xunit;

namespace EnergiaSustentavel.Tests;

/// <summary>
/// Teste de integração do LeiturasController.
/// Valida que a listagem paginada de leituras retorna o status code 200.
/// </summary>
public class LeiturasControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LeiturasControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/leituras";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se o status code é 200 (sucesso)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_FiltrandoPorEquipamento_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/leituras?equipamentoId=1&pagina=1&tamanhoPagina=10";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

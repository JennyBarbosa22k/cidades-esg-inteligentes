using System.Net;
using Xunit;

namespace EnergiaSustentavel.Tests;

/// <summary>
/// Teste de integração do AlertasController.
/// Valida que a listagem paginada de alertas retorna o status code 200.
/// </summary>
public class AlertasControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertasControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/alertas";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se o status code é 200 (sucesso)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_FiltrandoNaoResolvidos_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/alertas?resolvido=false";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

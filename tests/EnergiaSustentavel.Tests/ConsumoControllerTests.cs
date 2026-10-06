using System.Net;
using Xunit;

namespace EnergiaSustentavel.Tests;

/// <summary>
/// Teste de integração do ConsumoController.
/// Valida que os endpoints analíticos (dashboard e resumo) retornam status code 200.
/// </summary>
public class ConsumoControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ConsumoControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboard_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/consumo/dashboard";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se o status code é 200 (sucesso)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetResumo_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/consumo/resumo";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

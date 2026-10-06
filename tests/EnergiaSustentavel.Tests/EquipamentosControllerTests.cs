using System.Net;
using Xunit;

namespace EnergiaSustentavel.Tests;

/// <summary>
/// Teste de integração do EquipamentosController.
/// Valida que a listagem paginada retorna o status code 200.
/// </summary>
public class EquipamentosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EquipamentosControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/equipamentos";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se o status code é 200 (sucesso)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ComPaginacao_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/equipamentos?pagina=1&tamanhoPagina=5";

        // Act
        var response = await _client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

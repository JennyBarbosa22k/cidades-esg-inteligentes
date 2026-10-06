using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EnergiaSustentavel.Tests;

/// <summary>
/// Teste de integração do AuthController.
/// Valida que o login com credenciais válidas (usuário semeado) retorna status 200.
/// </summary>
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_ReturnsHttpStatusCode200()
    {
        // Arrange
        var request = "/api/auth/login";
        var credenciais = new
        {
            email = "admin@energia.com",
            senha = "Admin@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync(request, credenciais);

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se o status code é 200 (sucesso)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_ReturnsUnauthorized()
    {
        // Arrange
        var request = "/api/auth/login";
        var credenciais = new
        {
            email = "admin@energia.com",
            senha = "senhaErrada123"
        };

        // Act
        var response = await _client.PostAsJsonAsync(request, credenciais);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

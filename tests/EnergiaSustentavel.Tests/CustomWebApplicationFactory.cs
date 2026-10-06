using EnergiaSustentavel.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnergiaSustentavel.Tests;

/// <summary>
/// Fábrica de aplicação para os testes de integração. Substitui o provedor de banco
/// (SQL Server) por um banco em memória (EF Core InMemory), garantindo que os testes
/// rodem sem depender de um banco real. O seed inicial é executado pelo próprio
/// Program na inicialização, populando dados para os testes.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o registro do DbContext que usa SQL Server.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Registra o DbContext usando banco em memória, com nome único por instância.
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
        });

        builder.UseEnvironment("Development");
    }
}

using EnergiaSustentavel.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnergiaSustentavel.Tests;


public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Nome gerado uma única vez por instância da fábrica, para que todas as
        // requisições e o seed usem o mesmo banco em memória.
        var dbName = "TestDb_" + Guid.NewGuid();

        builder.ConfigureServices(services =>
        {
            // Remove o registro do DbContext que usa SQL Server.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // Registra o DbContext usando banco em memória com o nome fixo acima.
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
        });

        builder.UseEnvironment("Development");
    }
}

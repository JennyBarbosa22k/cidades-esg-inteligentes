using EnergiaSustentavel.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EnergiaSustentavel.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o banco SQL Server
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Cria banco em memória para os testes
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
            });
        });

        builder.UseEnvironment("Development");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Cria o usuário de teste depois que o banco foi criado
        using var scope = host.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Garante que o banco exista
        db.Database.EnsureCreated();

        // Procura a entidade Usuario no modelo
        var entityType = db.Model
            .GetEntityTypes()
            .FirstOrDefault(e => e.ClrType.Name == "Usuario");

        if (entityType != null)
        {
            var usuarioExistente = db
                .Set(entityType.ClrType)
                .Cast<object>()
                .FirstOrDefault();

            if (usuarioExistente == null)
            {
                var usuario = Activator.CreateInstance(entityType.ClrType);

                if (usuario != null)
                {
                    var tipoUsuario = usuario.GetType();

                    tipoUsuario.GetProperty("Email")?
                        .SetValue(usuario, "admin@energia.com");

                    tipoUsuario.GetProperty("SenhaHash")?
                        .SetValue(
                            usuario,
                            BCrypt.Net.BCrypt.HashPassword("Admin@123")
                        );

                    tipoUsuario.GetProperty("Perfil")?
                        .SetValue(usuario, "Admin");

                    db.Add(usuario);
                    db.SaveChanges();
                }
            }
        }

        return host;
    }
}

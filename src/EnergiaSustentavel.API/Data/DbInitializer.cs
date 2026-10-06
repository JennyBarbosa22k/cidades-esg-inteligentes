using EnergiaSustentavel.API.Models;

namespace EnergiaSustentavel.API.Data;

/// <summary>
/// Popula o banco com dados iniciais (usuário administrador e alguns equipamentos
/// e leituras de exemplo). É idempotente: só insere se ainda não houver dados.
/// </summary>
public static class DbInitializer
{
    public const string AdminEmail = "admin@energia.com";
    public const string AdminSenha = "Admin@123";

    public static void Seed(AppDbContext db)
    {
        // Usuário administrador
        if (!db.Usuarios.Any())
        {
            db.Usuarios.Add(new Usuario
            {
                Nome = "Administrador",
                Email = AdminEmail,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(AdminSenha),
                Perfil = "Admin"
            });
            db.SaveChanges();
        }

        // Equipamentos + leituras de exemplo
        if (!db.Equipamentos.Any())
        {
            var ar = new Equipamento
            {
                Nome = "Ar-condicionado Central",
                Localizacao = "Andar 3 - Bloco A",
                PotenciaWatts = 3500,
                LimiteConsumoKwh = 15,
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };
            var servidor = new Equipamento
            {
                Nome = "Rack de Servidores",
                Localizacao = "Datacenter - Subsolo",
                PotenciaWatts = 8000,
                LimiteConsumoKwh = 40,
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };
            var iluminacao = new Equipamento
            {
                Nome = "Iluminação Estacionamento",
                Localizacao = "Térreo - Externo",
                PotenciaWatts = 1200,
                LimiteConsumoKwh = 8,
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };

            db.Equipamentos.AddRange(ar, servidor, iluminacao);
            db.SaveChanges();

            db.Leituras.AddRange(
                new LeituraConsumo { EquipamentoId = ar.Id, ConsumoKwh = 12.5m, DataLeitura = DateTime.UtcNow.AddHours(-5), Fonte = "Sensor IoT" },
                new LeituraConsumo { EquipamentoId = ar.Id, ConsumoKwh = 14.2m, DataLeitura = DateTime.UtcNow.AddHours(-3), Fonte = "Sensor IoT" },
                new LeituraConsumo { EquipamentoId = servidor.Id, ConsumoKwh = 35.0m, DataLeitura = DateTime.UtcNow.AddHours(-4), Fonte = "Sensor IoT" },
                new LeituraConsumo { EquipamentoId = iluminacao.Id, ConsumoKwh = 6.3m, DataLeitura = DateTime.UtcNow.AddHours(-2), Fonte = "Sensor IoT" }
            );
            db.SaveChanges();
        }
    }
}

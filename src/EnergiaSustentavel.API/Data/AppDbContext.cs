using EnergiaSustentavel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergiaSustentavel.API.Data;

/// <summary>
/// Contexto do Entity Framework Core. Mapeia as entidades para tabelas e define
/// restrições, índices e relacionamentos via Fluent API.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Equipamento> Equipamentos => Set<Equipamento>();
    public DbSet<LeituraConsumo> Leituras => Set<LeituraConsumo>();
    public DbSet<Alerta> Alertas => Set<Alerta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Usuario ----------
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(u => u.Id);
            e.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(150);
            e.Property(u => u.SenhaHash).IsRequired();
            e.Property(u => u.Perfil).IsRequired().HasMaxLength(20);
            e.HasIndex(u => u.Email).IsUnique();
        });

        // ---------- Equipamento ----------
        modelBuilder.Entity<Equipamento>(e =>
        {
            e.ToTable("Equipamentos");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(100);
            e.Property(x => x.Localizacao).IsRequired().HasMaxLength(150);
            e.Property(x => x.PotenciaWatts).HasPrecision(12, 2);
            e.Property(x => x.LimiteConsumoKwh).HasPrecision(12, 2);
            e.Property(x => x.Ativo).IsRequired();
            e.Property(x => x.CriadoEm).IsRequired();
            e.HasIndex(x => x.Nome);
        });

        // ---------- LeituraConsumo ----------
        modelBuilder.Entity<LeituraConsumo>(e =>
        {
            e.ToTable("LeiturasConsumo");
            e.HasKey(x => x.Id);
            e.Property(x => x.ConsumoKwh).HasPrecision(12, 2);
            e.Property(x => x.DataLeitura).IsRequired();
            e.Property(x => x.Fonte).IsRequired().HasMaxLength(50);

            e.HasOne(x => x.Equipamento)
             .WithMany(eq => eq.Leituras)
             .HasForeignKey(x => x.EquipamentoId)
             .OnDelete(DeleteBehavior.Cascade);

            // Índice para otimizar consultas/paginação por equipamento e data.
            e.HasIndex(x => new { x.EquipamentoId, x.DataLeitura });
        });

        // ---------- Alerta ----------
        modelBuilder.Entity<Alerta>(e =>
        {
            e.ToTable("Alertas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Mensagem).IsRequired().HasMaxLength(300);
            e.Property(x => x.ConsumoRegistrado).HasPrecision(12, 2);
            e.Property(x => x.LimiteUltrapassado).HasPrecision(12, 2);
            e.Property(x => x.Severidade).IsRequired().HasMaxLength(20);
            e.Property(x => x.Resolvido).IsRequired();
            e.Property(x => x.CriadoEm).IsRequired();

            e.HasOne(x => x.Equipamento)
             .WithMany(eq => eq.Alertas)
             .HasForeignKey(x => x.EquipamentoId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.Resolvido);
        });
    }
}

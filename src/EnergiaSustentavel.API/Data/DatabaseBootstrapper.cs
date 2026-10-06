using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace EnergiaSustentavel.API.Data;

/// <summary>
/// Garante que o schema e as tabelas existam no PostgreSQL. É idempotente:
/// se as tabelas já existirem (reinício da aplicação), o erro "tabela duplicada" é ignorado.
/// O parâmetro <c>schema</c> permite que staging e produção compartilhem a mesma instância
/// de banco, cada um em um schema próprio (ex.: "staging" e "producao").
/// </summary>
public static class DatabaseBootstrapper
{
    public static void EnsureTables(AppDbContext db, string? schema)
    {
        if (!string.IsNullOrWhiteSpace(schema))
        {
            if (!schema.All(c => char.IsLetterOrDigit(c) || c == '_'))
                throw new InvalidOperationException("Database:Schema inválido (use apenas letras, números e _).");

#pragma warning disable EF1002 // o nome do schema vem de configuração do operador e é validado acima
            db.Database.ExecuteSqlRaw($"CREATE SCHEMA IF NOT EXISTS \"{schema}\"");
#pragma warning restore EF1002
        }

        var creator = db.GetService<IRelationalDatabaseCreator>();
        try
        {
            creator.CreateTables();
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.DuplicateTable)
        {
            // Tabelas já existem: nada a fazer.
        }
    }
}

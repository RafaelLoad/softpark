using Dapper;
using Microsoft.Data.Sqlite;
using Softpark.Domain.Entities;
using Softpark.Domain.Interfaces;
using Softpark.Infrastructure.Data;

namespace Softpark.Infrastructure.Repositories;

public class UsuarioRepository(IDbConnectionFactory connectionFactory) : IUsuarioRepository
{
    public async Task<(IEnumerable<Usuario> Usuarios, int Total)> ListarAsync(int pagina, int tamanhoPagina)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        var total = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM usuario");

        var offset = (pagina - 1) * tamanhoPagina;
        var rows = await connection.QueryAsync<UsuarioComPerfilRow>(
            @"SELECT u.Id, u.Usuario, u.Status, p.Id AS PerfilId, p.Perfil
              FROM usuario u
              LEFT JOIN usuario_perfil p ON u.Id = p.UsuarioId
              WHERE u.Id IN (SELECT Id FROM usuario ORDER BY Id LIMIT @TamanhoPagina OFFSET @Offset)
              ORDER BY u.Id",
            new { Offset = offset, TamanhoPagina = tamanhoPagina });

        var usuarios = rows
            .GroupBy(r => new { r.Id, r.Usuario, r.Status })
            .Select(g => Usuario.Carregar(
                (int)g.Key.Id,
                g.Key.Usuario,
                g.Key.Status != 0,
                g.Where(r => r.PerfilId is not null)
                 .Select(r => UsuarioPerfil.Carregar((int)r.PerfilId!, (int)g.Key.Id, r.Perfil!))
            ));

        return (usuarios, total);
    }

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        var rows = await connection.QueryAsync<UsuarioComPerfilRow>(
            @"SELECT u.Id, u.Usuario, u.Status, p.Id AS PerfilId, p.Perfil
              FROM usuario u
              LEFT JOIN usuario_perfil p ON u.Id = p.UsuarioId
              WHERE u.Id = @Id",
            new { Id = id });

        var grupo = rows.GroupBy(r => new { r.Id, r.Usuario, r.Status }).FirstOrDefault();
        if (grupo is null)
            return null;

        return Usuario.Carregar(
            (int)grupo.Key.Id,
            grupo.Key.Usuario,
            grupo.Key.Status != 0,
            grupo.Where(r => r.PerfilId is not null)
                 .Select(r => UsuarioPerfil.Carregar((int)r.PerfilId!, (int)grupo.Key.Id, r.Perfil!))
        );
    }

    public async Task<bool> ExistePorNomeAsync(string nomeUsuario)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        var count = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM usuario WHERE Usuario = @Usuario",
            new { Usuario = nomeUsuario });

        return count > 0;
    }

    public async Task<int> CriarAsync(Usuario usuario)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var identitySql = connection is SqliteConnection
                ? "SELECT last_insert_rowid();"
                : "SELECT SCOPE_IDENTITY();";

            var usuarioId = await connection.ExecuteScalarAsync<int>(
                $@"INSERT INTO usuario (Usuario, Status)
                   VALUES (@Usuario, @Status);
                   {identitySql}",
                new { Usuario = usuario.NomeUsuario, usuario.Status },
                transaction);

            foreach (var perfil in usuario.Perfis)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO usuario_perfil (UsuarioId, Perfil) VALUES (@UsuarioId, @Perfil)",
                    new { UsuarioId = usuarioId, Perfil = perfil.Perfil },
                    transaction);
            }

            transaction.Commit();
            return usuarioId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(
                "UPDATE usuario SET Usuario = @Usuario, Status = @Status WHERE Id = @Id",
                new { Usuario = usuario.NomeUsuario, usuario.Status, usuario.Id },
                transaction);

            await connection.ExecuteAsync(
                "DELETE FROM usuario_perfil WHERE UsuarioId = @UsuarioId",
                new { UsuarioId = usuario.Id },
                transaction);

            foreach (var perfil in usuario.Perfis)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO usuario_perfil (UsuarioId, Perfil) VALUES (@UsuarioId, @Perfil)",
                    new { UsuarioId = usuario.Id, Perfil = perfil.Perfil },
                    transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private record UsuarioComPerfilRow(long Id, string Usuario, long Status, long? PerfilId, string? Perfil);
}

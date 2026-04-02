using Dapper;

namespace Softpark.Infrastructure.Data;

public class DatabaseInitializer(IDbConnectionFactory connectionFactory)
{
    public void Initialize()
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS usuario (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Usuario TEXT NOT NULL,
                Status INTEGER NOT NULL DEFAULT 1
            )
        """);

        connection.Execute("""
            CREATE TABLE IF NOT EXISTS usuario_perfil (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UsuarioId INTEGER NOT NULL,
                Perfil TEXT NOT NULL,
                FOREIGN KEY (UsuarioId) REFERENCES usuario(Id)
            )
        """);

        var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM usuario");
        if (count > 0) return;

        connection.Execute(
            "INSERT INTO usuario (Usuario, Status) VALUES (@Usuario, @Status)",
            new { Usuario = "joao silva", Status = 1 });

        connection.Execute(
            "INSERT INTO usuario (Usuario, Status) VALUES (@Usuario, @Status)",
            new { Usuario = "maria santos", Status = 1 });

        connection.Execute(
            "INSERT INTO usuario (Usuario, Status) VALUES (@Usuario, @Status)",
            new { Usuario = "pedro oliveira", Status = 0 });

        connection.Execute(
            "INSERT INTO usuario_perfil (UsuarioId, Perfil) VALUES (@UsuarioId, @Perfil)",
            new[]
            {
                new { UsuarioId = 1, Perfil = "Administrador" },
                new { UsuarioId = 1, Perfil = "Gerente" },
                new { UsuarioId = 2, Perfil = "Usuário" },
                new { UsuarioId = 3, Perfil = "Usuário" },
                new { UsuarioId = 3, Perfil = "Consultor" }
            });
    }
}

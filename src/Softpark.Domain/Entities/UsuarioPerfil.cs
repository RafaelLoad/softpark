using Softpark.Domain.Exceptions;

namespace Softpark.Domain.Entities;

public class UsuarioPerfil
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public string Perfil { get; private set; } = string.Empty;

    private UsuarioPerfil() { }

    public UsuarioPerfil(string perfil)
    {
        if (string.IsNullOrWhiteSpace(perfil))
            throw new DomainException("O perfil não pode ser vazio.");

        Perfil = perfil.Trim();
    }

    public void DefinirUsuarioId(int usuarioId)
    {
        UsuarioId = usuarioId;
    }

    public static UsuarioPerfil Carregar(int id, int usuarioId, string perfil)
    {
        return new UsuarioPerfil
        {
            Id = id,
            UsuarioId = usuarioId,
            Perfil = perfil
        };
    }
}

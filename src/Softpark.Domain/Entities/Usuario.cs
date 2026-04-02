using Softpark.Domain.Enums;
using Softpark.Domain.Exceptions;

namespace Softpark.Domain.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string NomeUsuario { get; private set; } = string.Empty;
    public bool Status { get; private set; }

    private readonly List<UsuarioPerfil> _perfis = new();
    public IReadOnlyCollection<UsuarioPerfil> Perfis => _perfis.AsReadOnly();

    private Usuario() { }

    public Usuario(string nomeUsuario, bool status, IEnumerable<string> perfis)
    {
        var listaPerfis = perfis?.ToList() ?? new List<string>();
        Validar(nomeUsuario, listaPerfis);

        NomeUsuario = nomeUsuario.Trim();
        Status = status;

        foreach (var perfil in listaPerfis)
            _perfis.Add(new UsuarioPerfil(perfil));
    }

    public void Atualizar(string nomeUsuario, bool status, IEnumerable<string> perfis)
    {
        var listaPerfis = perfis?.ToList() ?? new List<string>();
        Validar(nomeUsuario, listaPerfis);

        NomeUsuario = nomeUsuario.Trim();
        Status = status;

        _perfis.Clear();
        foreach (var perfil in listaPerfis)
            _perfis.Add(new UsuarioPerfil(perfil));
    }

    public static Usuario Carregar(int id, string nomeUsuario, bool status, IEnumerable<UsuarioPerfil> perfis)
    {
        var usuario = new Usuario
        {
            Id = id,
            NomeUsuario = nomeUsuario,
            Status = status
        };
        usuario._perfis.AddRange(perfis);
        return usuario;
    }

    private static void Validar(string nomeUsuario, List<string> perfis)
    {
        if (string.IsNullOrWhiteSpace(nomeUsuario))
            throw new DomainException("O nome do usuário é obrigatório.");

        if (perfis.Count == 0)
            throw new DomainException("O usuário deve ter pelo menos um perfil.");

        if (perfis.Any(string.IsNullOrWhiteSpace))
            throw new DomainException("Nenhum perfil pode ser vazio.");

        var perfisValidos = Enum.GetNames<PerfilEnum>();
        var invalidos = perfis
            .Where(p => !perfisValidos.Contains(p.Trim(), StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (invalidos.Count > 0)
            throw new DomainException(
                $"Perfil(is) inválido(s): {string.Join(", ", invalidos)}. " +
                $"Valores permitidos: {string.Join(", ", perfisValidos)}.");
    }
}

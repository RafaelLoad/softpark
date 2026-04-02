using Softpark.Domain.Entities;

namespace Softpark.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<(IEnumerable<Usuario> Usuarios, int Total)> ListarAsync(int pagina, int tamanhoPagina);
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<bool> ExistePorNomeAsync(string nomeUsuario);
    Task<int> CriarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
}

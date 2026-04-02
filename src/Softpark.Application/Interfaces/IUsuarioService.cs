using Softpark.Application.DTOs;

namespace Softpark.Application.Interfaces;

public interface IUsuarioService
{
    Task<PagedDto<UsuarioDto>> ListarAsync(int pagina, int tamanhoPagina);
    Task<UsuarioDto?> ObterPorIdAsync(int id);
    Task<UsuarioDto> CriarAsync(CriarUsuarioDto request);
    Task<UsuarioDto> AtualizarAsync(int id, AtualizarUsuarioDto request);
}

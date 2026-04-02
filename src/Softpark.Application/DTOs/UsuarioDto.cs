namespace Softpark.Application.DTOs;

public record CriarUsuarioDto(string Usuario, bool Status, List<string> Perfis);

public record AtualizarUsuarioDto(string Usuario, bool Status, List<string> Perfis);

public record UsuarioDto(int Id, string Usuario, bool Status, List<string> Perfis);

public record PagedDto<T>(IEnumerable<T> Items, int Total, int Pagina, int TamanhoPagina);

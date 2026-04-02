namespace Softpark.Application.DTOs;

public record LoginDto(string Usuario, string Senha);

public record TokenDto(string Token);

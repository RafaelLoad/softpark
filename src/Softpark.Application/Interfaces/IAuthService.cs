using Softpark.Application.DTOs;

namespace Softpark.Application.Interfaces;

public interface IAuthService
{
    string? Autenticar(LoginDto request);
}

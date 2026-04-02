using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Softpark.Application.DTOs;
using Softpark.Application.Interfaces;

namespace Softpark.Infrastructure.Auth;

public class JwtAuthService(IOptions<JwtSettings> settings) : IAuthService
{
    private readonly JwtSettings _settings = settings.Value;

    private const string Usuario = "admin";
    private const string Senha = "123";

    public string? Autenticar(LoginDto request)
    {
        if (request.Usuario != Usuario || request.Senha != Senha)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, request.Usuario)
            }),
            Expires = DateTime.UtcNow.AddHours(_settings.ExpiracaoHoras),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

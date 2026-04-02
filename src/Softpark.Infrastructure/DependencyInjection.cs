using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Softpark.Application.Interfaces;
using Softpark.Application.UseCases;
using Softpark.Domain.Interfaces;
using Softpark.Infrastructure.Auth;
using Softpark.Infrastructure.Data;
using Softpark.Infrastructure.Repositories;

namespace Softpark.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(options =>
        {
            var section = configuration.GetSection("Jwt");
            options.Secret = section["Secret"] ?? string.Empty;
            options.Issuer = section["Issuer"] ?? string.Empty;
            options.Audience = section["Audience"] ?? string.Empty;
            options.ExpiracaoHoras = int.TryParse(section["ExpiracaoHoras"], out var h) ? h : 1;
        });

        var useSqlite = configuration.GetValue<bool>("UseSqlite");

        if (useSqlite)
        {
            services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
            services.AddSingleton<DatabaseInitializer>();
        }
        else
        {
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        }

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAuthService, JwtAuthService>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UsuarioService>();

        return services;
    }
}

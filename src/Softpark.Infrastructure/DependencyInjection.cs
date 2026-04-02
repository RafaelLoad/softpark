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
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        AddApplicationSetup(services);
        AddDomainSetup(services);
        AddInfraSetup(services, configuration);
        AddDatabaseSetup(services, configuration);

        return services;
    }

    private static void AddApplicationSetup(IServiceCollection services)
    {
        services
            .AddScoped<IUsuarioService, UsuarioService>();
    }

    private static void AddDomainSetup(IServiceCollection services)
    {
        services
            .AddScoped<IUsuarioRepository, UsuarioRepository>();
    }

    private static void AddInfraSetup(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(options =>
        {
            var section = configuration.GetSection("Jwt");
            options.Secret = section["Secret"] ?? string.Empty;
            options.Issuer = section["Issuer"] ?? string.Empty;
            options.Audience = section["Audience"] ?? string.Empty;
            options.ExpiracaoHoras = int.TryParse(section["ExpiracaoHoras"], out var horas) ? horas : 1;
        });

        services
            .AddScoped<IAuthService, JwtAuthService>();
    }

    private static void AddDatabaseSetup(IServiceCollection services, IConfiguration configuration)
    {
        var useSqlite = configuration.GetValue<bool>("UseSqlite");

        if (useSqlite)
        {
            services
                .AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>()
                .AddSingleton<DatabaseInitializer>();
        }
        else
        {
            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        }
    }
}

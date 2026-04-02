using Microsoft.Extensions.Logging;
using Softpark.Application.DTOs;
using Softpark.Domain.Entities;
using Softpark.Domain.Exceptions;
using NotFoundException = Softpark.Domain.Exceptions.NotFoundException;
using Softpark.Domain.Interfaces;

namespace Softpark.Application.UseCases;

public class UsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(IUsuarioRepository repository, ILogger<UsuarioService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedDto<UsuarioDto>> ListarAsync(int pagina, int tamanhoPagina)
    {
        if (pagina < 1) pagina = 1;
        if (tamanhoPagina < 1) tamanhoPagina = 10;
        if (tamanhoPagina > 100) tamanhoPagina = 100;

        _logger.LogInformation("Listando usuários - Página: {Pagina}, Tamanho: {Tamanho}", pagina, tamanhoPagina);

        var (usuarios, total) = await _repository.ListarAsync(pagina, tamanhoPagina);

        var items = usuarios.Select(u => new UsuarioDto(
            u.Id,
            u.NomeUsuario,
            u.Status,
            u.Perfis.Select(p => p.Perfil).ToList()
        ));

        return new PagedDto<UsuarioDto>(items, total, pagina, tamanhoPagina);
    }

    public async Task<UsuarioDto?> ObterPorIdAsync(int id)
    {
        _logger.LogInformation("Obtendo usuário por ID: {Id}", id);

        var usuario = await _repository.ObterPorIdAsync(id);
        if (usuario is null)
            return null;

        return new UsuarioDto(
            usuario.Id,
            usuario.NomeUsuario,
            usuario.Status,
            usuario.Perfis.Select(p => p.Perfil).ToList()
        );
    }

    public async Task<UsuarioDto> CriarAsync(CriarUsuarioDto request)
    {
        try
        {
            _logger.LogInformation("Criando usuário: {Usuario}", request.Usuario);

            if (await _repository.ExistePorNomeAsync(request.Usuario))
                throw new DomainException($"Já existe um usuário com o nome '{request.Usuario}'.");

            var id = await _repository.CriarAsync(
                new Usuario(
                    request.Usuario, 
                    request.Status, 
                    request.Perfis));

            var usuario = await _repository.ObterPorIdAsync(id);

            return new UsuarioDto(
                usuario!.Id,
                usuario.NomeUsuario,
                usuario.Status,
                usuario.Perfis.Select(p => p.Perfil).ToList()
            );
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Erro ao criar usuário: {Mensagem}", ex.Message);
            throw;
        }
    }

    public async Task<UsuarioDto> AtualizarAsync(int id, AtualizarUsuarioDto request)
    {
        try
        {
            _logger.LogInformation("Atualizando usuário ID: {Id}", id);

            var usuario = await _repository.ObterPorIdAsync(id)
                ?? throw new NotFoundException($"Usuário com ID {id} não encontrado.");

            usuario.Atualizar(request.Usuario, request.Status, request.Perfis);
            await _repository.AtualizarAsync(usuario);
            var atualizado = await _repository.ObterPorIdAsync(id);

            return new UsuarioDto(
                atualizado!.Id,
                atualizado.NomeUsuario,
                atualizado.Status,
                atualizado.Perfis.Select(p => p.Perfil).ToList()
            );
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Erro ao atualizar usuário {Id}: {Mensagem}", id, ex.Message);
            throw;
        }
    }
}

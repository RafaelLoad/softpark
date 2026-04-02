using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Softpark.Application.DTOs;
using Softpark.Application.Interfaces;

namespace Softpark.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedDto<UsuarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10) => Ok(await _usuarioService.ListarAsync(pagina, tamanhoPagina));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id);

        if (usuario is null)
            return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });

        return Ok(usuario);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto request)
    {
        var usuario = await _usuarioService.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDto request)
    {
        return Ok(await _usuarioService.AtualizarAsync(id, request));
    }
}

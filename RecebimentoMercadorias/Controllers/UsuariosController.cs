using Microsoft.AspNetCore.Mvc;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Usuario>> Get()
    {
        return Ok(_usuarioService.ObterTodos());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Usuario> GetById(int id)
    {
        var usuario = _usuarioService.ObterPorId(id);

        if (usuario is null)
        {
            return NotFound();
        }

        return Ok(usuario);
    }

    [HttpPost]
    public ActionResult<Usuario> Post([FromBody] CreateUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Cargo))
        {
            return BadRequest("Nome, e-mail e cargo são obrigatórios.");
        }

        var usuario = _usuarioService.Criar(
            request.Nome,
            request.Email,
            request.Cargo,
            request.Ativo);

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Usuario> Put(int id, [FromBody] UpdateUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Cargo))
        {
            return BadRequest("Nome, e-mail e cargo são obrigatórios.");
        }

        var usuarioAtualizado = _usuarioService.Atualizar(
            id,
            request.Nome,
            request.Email,
            request.Cargo,
            request.Ativo);

        if (usuarioAtualizado is null)
        {
            return NotFound();
        }

        return Ok(usuarioAtualizado);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var removido = _usuarioService.Remover(id);

        if (!removido)
        {
            return NotFound();
        }

        return NoContent();
    }
}

public sealed class CreateUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public sealed class UpdateUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get()
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Post([FromBody] CreateUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Cargo))
        {
            return BadRequest("Nome, e-mail e cargo são obrigatórios.");
        }

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = request.Email.Trim(),
            Cargo = request.Cargo.Trim(),
            Ativo = request.Ativo,
            DataCadastro = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Usuario>> Put(int id, [FromBody] UpdateUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Cargo))
        {
            return BadRequest("Nome, e-mail e cargo são obrigatórios.");
        }

        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        usuario.Nome = request.Nome.Trim();
        usuario.Email = request.Email.Trim();
        usuario.Cargo = request.Cargo.Trim();
        usuario.Ativo = request.Ativo;

        await _context.SaveChangesAsync();

        return Ok(usuario);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

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
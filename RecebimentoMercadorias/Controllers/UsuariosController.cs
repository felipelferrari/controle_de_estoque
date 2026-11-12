using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;
using RecebimentoMercadorias.Services;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StockUser")]
public sealed class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SupabaseAdminClient _supabaseAdmin;

    public UsuariosController(AppDbContext context, SupabaseAdminClient supabaseAdmin)
    {
        _context = context;
        _supabaseAdmin = supabaseAdmin;
    }

    [HttpGet]
    [Authorize(Policy = "Managers")]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get()
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Managers")]
    public async Task<ActionResult<Usuario>> GetById(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        return Ok(usuario);
    }

    [HttpGet("me")]
    public async Task<ActionResult<CurrentUsuarioResponse>> GetMe()
    {
        if (!Guid.TryParse(User.FindFirstValue("sub"), out var authUserId))
        {
            return Unauthorized();
        }

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .Where(user => user.AuthUserId == authUserId && user.Ativo)
            .Select(user => new CurrentUsuarioResponse(user.Id, user.Nome, user.Email, user.Cargo))
            .SingleOrDefaultAsync();

        return usuario is null ? Forbid() : Ok(usuario);
    }

    [HttpPost]
    [Authorize(Policy = "Managers")]
    public async Task<ActionResult<Usuario>> Post([FromBody] CreateUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Cargo))
        {
            return BadRequest("Nome, e-mail e cargo são obrigatórios.");
        }

        var nome = request.Nome.Trim();
        var email = request.Email.Trim();
        var cargo = request.Cargo.Trim().ToUpperInvariant();
        if (!UsuarioRoles.AllowedRoles.Contains(cargo))
        {
            return BadRequest("Cargo inválido.");
        }

        if (await _context.Usuarios.AnyAsync(user => user.Email.ToLower() == email.ToLower()))
        {
            return Conflict("Já existe um usuário cadastrado com este e-mail.");
        }

        Guid authUserId;
        try
        {
            authUserId = await _supabaseAdmin.InviteUserAsync(email, nome, HttpContext.RequestAborted);
        }
        catch (SupabaseAdminException exception)
        {
            return StatusCode((int)exception.StatusCode, new { message = exception.Message });
        }

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Cargo = cargo,
            Ativo = request.Ativo,
            DataCadastro = DateTime.UtcNow,
            AuthUserId = authUserId
        };

        _context.Usuarios.Add(usuario);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "O convite foi enviado, mas não foi possível salvar o perfil do usuário.");
        }

        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Managers")]
    public async Task<ActionResult<Usuario>> Put(int id, [FromBody] UpdateUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Cargo))
        {
            return BadRequest("Nome, e-mail e cargo são obrigatórios.");
        }

        if (!UsuarioRoles.AllowedRoles.Contains(request.Cargo.Trim().ToUpperInvariant()))
        {
            return BadRequest("Cargo inválido.");
        }

        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        if (!string.Equals(usuario.Email, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("O e-mail vinculado à conta Supabase não pode ser alterado por esta operação.");
        }

        usuario.Nome = request.Nome.Trim();
        usuario.Cargo = request.Cargo.Trim();
        usuario.Ativo = request.Ativo;

        await _context.SaveChangesAsync();

        return Ok(usuario);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CEO")]
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

public sealed record CurrentUsuarioResponse(int Id, string Nome, string Email, string Cargo);

public sealed class CreateUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}

public static class UsuarioRoles
{
    public static readonly HashSet<string> AllowedRoles =
        ["TRABALHADOR", "FILIAL", "GERENTE", "CONFERENTE", "CEO"];
}

public sealed class UpdateUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
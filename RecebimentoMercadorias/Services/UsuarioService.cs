using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Services;

public sealed class UsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context)
    {
        _context = context;
    }

    public List<Usuario> ObterTodos()
    {
        return _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .ToList();
    }

    public Usuario? ObterPorId(int id)
    {
        return _context.Usuarios
            .FirstOrDefault(u => u.Id == id);
    }

    public Usuario Criar(string nome, string email, string cargo, bool ativo = true)
    {
        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Email = email.Trim(),
            Cargo = cargo.Trim(),
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        return usuario;
    }

    public Usuario? Atualizar(int id, string nome, string email, string cargo, bool ativo)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario is null) return null;

        usuario.Nome = nome.Trim();
        usuario.Email = email.Trim();
        usuario.Cargo = cargo.Trim();
        usuario.Ativo = ativo;

        _context.SaveChanges();

        return usuario;
    }

    public bool Remover(int id)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario is null) return false;

        _context.Usuarios.Remove(usuario);
        _context.SaveChanges();

        return true;
    }

    public int ObterQuantidadeAtivos()
    {
        return _context.Usuarios.Count(u => u.Ativo);
    }
}
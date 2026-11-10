using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Services;

public sealed class CategoriaService
{
    private readonly AppDbContext _context;

    public CategoriaService(AppDbContext context)
    {
        _context = context;
    }

    public List<Categoria> ObterTodas()
    {
        return _context.Categorias
            .AsNoTracking()
            .OrderBy(categoria => categoria.Nome)
            .ToList();
    }

    public Categoria? ObterPorId(int id)
    {
        return _context.Categorias
            .AsNoTracking()
            .FirstOrDefault(categoria => categoria.Id == id);
    }

    public Categoria Criar(string nome, string? descricao)
    {
        var categoria = new Categoria
        {
            Nome = nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim(),
        };

        _context.Categorias.Add(categoria);
        _context.SaveChanges();

        return categoria;
    }

    public Categoria? Atualizar(int id, string nome, string? descricao)
    {
        var categoria = _context.Categorias.FirstOrDefault(item => item.Id == id);
        if (categoria is null) return null;

        categoria.Nome = nome.Trim();
        categoria.Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        _context.SaveChanges();

        return categoria;
    }

    public bool EstaEmUso(int id)
    {
        return _context.Produtos.Any(produto => produto.IdCategoria == id);
    }

    public bool Remover(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(item => item.Id == id);
        if (categoria is null) return false;

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();

        return true;
    }
}using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Services;

public sealed class CategoriaService
{
    private readonly AppDbContext _context;

    public CategoriaService(AppDbContext context)
    {
        _context = context;
    }

    public List<Categoria> ObterTodos()
    {
        return _context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToList();
    }

    public Categoria? ObterPorId(int id)
    {
        return _context.Categorias.FirstOrDefault(c => c.Id == id);
    }

    public Categoria Criar(string nome, string? descricao, bool ativo = true)
    {
        var categoria = new Categoria
        {
            Nome = nome.Trim(),
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim(),
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow
        };

        _context.Categorias.Add(categoria);
        _context.SaveChanges();

        return categoria;
    }

    public Categoria? Atualizar(int id, string nome, string? descricao, bool ativo)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
        if (categoria is null) return null;

        categoria.Nome = nome.Trim();
        categoria.Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        categoria.Ativo = ativo;
        _context.SaveChanges();

        return categoria;
    }

    public bool Remover(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
        if (categoria is null) return false;

        if (_context.Produtos.Any(p => p.IdCategoria == id))
        {
            throw new InvalidOperationException("Não é possível excluir uma categoria vinculada a produtos.");
        }

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();

        return true;
    }
}
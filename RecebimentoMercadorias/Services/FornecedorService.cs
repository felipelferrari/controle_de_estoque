using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Services;

public sealed class FornecedorService
{
    private readonly AppDbContext _context;

    public FornecedorService(AppDbContext context)
    {
        _context = context;
    }

    public List<Fornecedor> ObterTodos()
    {
        return _context.Fornecedores
            .AsNoTracking()
            .OrderBy(f => f.Nome)
            .ToList();
    }

    public Fornecedor? ObterPorId(int id)
    {
        return _context.Fornecedores
            .FirstOrDefault(f => f.Id == id);
    }

    public Fornecedor Criar(string nome, string cnpj, string email, string telefone, bool ativo = true)
    {
        var fornecedor = new Fornecedor
        {
            Nome = nome.Trim(),
            Cnpj = cnpj.Trim(),
            Email = email.Trim(),
            Telefone = telefone.Trim(),
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow
        };

        _context.Fornecedores.Add(fornecedor);
        _context.SaveChanges();

        return fornecedor;
    }

    public Fornecedor? Atualizar(int id, string nome, string cnpj, string email, string telefone, bool ativo)
    {
        var fornecedor = _context.Fornecedores.FirstOrDefault(f => f.Id == id);

        if (fornecedor is null) return null;

        fornecedor.Nome = nome.Trim();
        fornecedor.Cnpj = cnpj.Trim();
        fornecedor.Email = email.Trim();
        fornecedor.Telefone = telefone.Trim();
        fornecedor.Ativo = ativo;

        _context.SaveChanges();

        return fornecedor;
    }

    public bool Remover(int id)
    {
        var fornecedor = _context.Fornecedores.FirstOrDefault(f => f.Id == id);

        if (fornecedor is null) return false;

        _context.Fornecedores.Remove(fornecedor);
        _context.SaveChanges();

        return true;
    }

    public int ObterQuantidadeAtivos()
    {
        return _context.Fornecedores.Count(f => f.Ativo);
    }
}
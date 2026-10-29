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
            .OrderBy(f => f.RazaoSocial)
            .ToList();
    }

    public Fornecedor? ObterPorId(int id)
    {
        return _context.Fornecedores
            .FirstOrDefault(f => f.Id == id);
    }

    public Fornecedor Criar(
        string razaoSocial,
        string? nomeFantasia,
        string cnpj,
        string? telefone,
        string? email,
        string? endereco,
        bool ativo = true)
    {
        var fornecedor = new Fornecedor
        {
            RazaoSocial = razaoSocial.Trim(),
            NomeFantasia = string.IsNullOrWhiteSpace(nomeFantasia) ? null : nomeFantasia.Trim(),
            Cnpj = cnpj.Trim(),
            Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            Endereco = string.IsNullOrWhiteSpace(endereco) ? null : endereco.Trim(),
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow
        };

        _context.Fornecedores.Add(fornecedor);
        _context.SaveChanges();

        return fornecedor;
    }

    public Fornecedor? Atualizar(
        int id,
        string razaoSocial,
        string? nomeFantasia,
        string cnpj,
        string? telefone,
        string? email,
        string? endereco,
        bool ativo)
    {
        var fornecedor = _context.Fornecedores.FirstOrDefault(f => f.Id == id);

        if (fornecedor is null) return null;

        fornecedor.RazaoSocial = razaoSocial.Trim();
        fornecedor.NomeFantasia = string.IsNullOrWhiteSpace(nomeFantasia) ? null : nomeFantasia.Trim();
        fornecedor.Cnpj = cnpj.Trim();
        fornecedor.Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim();
        fornecedor.Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        fornecedor.Endereco = string.IsNullOrWhiteSpace(endereco) ? null : endereco.Trim();
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
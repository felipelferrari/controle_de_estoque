namespace RecebimentoMercadorias.Services;

public sealed class Fornecedor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}

public sealed class FornecedorService
{
    private readonly List<Fornecedor> _fornecedores = new()
    {
        new()
        {
            Id = 1,
            Nome = "Distribuidora Alpha",
            Cnpj = "12.345.678/0001-90",
            Email = "contato@alpha.com.br",
            Telefone = "(11) 99999-1111",
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-15)
        },
        new()
        {
            Id = 2,
            Nome = "Logística Beta",
            Cnpj = "98.765.432/0001-11",
            Email = "vendas@beta.com.br",
            Telefone = "(21) 98888-2222",
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-7)
        }
    };

    public List<Fornecedor> ObterTodos()
    {
        return _fornecedores
            .OrderBy(f => f.Nome)
            .ToList();
    }

    public Fornecedor? ObterPorId(int id)
    {
        return _fornecedores.FirstOrDefault(f => f.Id == id);
    }

    public Fornecedor Criar(string nome, string cnpj, string email, string telefone, bool ativo = true)
    {
        var fornecedor = new Fornecedor
        {
            Id = _fornecedores.Any() ? _fornecedores.Max(f => f.Id) + 1 : 1,
            Nome = nome.Trim(),
            Cnpj = cnpj.Trim(),
            Email = email.Trim(),
            Telefone = telefone.Trim(),
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow
        };

        _fornecedores.Add(fornecedor);
        return fornecedor;
    }

    public Fornecedor? Atualizar(int id, string nome, string cnpj, string email, string telefone, bool ativo)
    {
        var fornecedor = _fornecedores.FirstOrDefault(f => f.Id == id);

        if (fornecedor is null)
        {
            return null;
        }

        fornecedor.Nome = nome.Trim();
        fornecedor.Cnpj = cnpj.Trim();
        fornecedor.Email = email.Trim();
        fornecedor.Telefone = telefone.Trim();
        fornecedor.Ativo = ativo;

        return fornecedor;
    }

    public bool Remover(int id)
    {
        var fornecedor = _fornecedores.FirstOrDefault(f => f.Id == id);

        if (fornecedor is null)
        {
            return false;
        }

        _fornecedores.Remove(fornecedor);
        return true;
    }

    public int ObterQuantidadeAtivos()
    {
        return _fornecedores.Count(f => f.Ativo);
    }
}

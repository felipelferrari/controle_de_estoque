namespace RecebimentoMercadorias.Services;

public sealed class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}

public sealed class UsuarioService
{
    private readonly List<Usuario> _usuarios = new()
    {
        new()
        {
            Id = 1,
            Nome = "Ana Souza",
            Email = "ana@empresa.com.br",
            Cargo = "Gerente de Estoque",
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-30)
        },
        new()
        {
            Id = 2,
            Nome = "Carlos Mendes",
            Email = "carlos@empresa.com.br",
            Cargo = "Operador",
            Ativo = true,
            DataCadastro = DateTime.UtcNow.AddDays(-12)
        }
    };

    public List<Usuario> ObterTodos()
    {
        return _usuarios
            .OrderBy(u => u.Nome)
            .ToList();
    }

    public Usuario? ObterPorId(int id)
    {
        return _usuarios.FirstOrDefault(u => u.Id == id);
    }

    public Usuario Criar(string nome, string email, string cargo, bool ativo = true)
    {
        var usuario = new Usuario
        {
            Id = _usuarios.Any() ? _usuarios.Max(u => u.Id) + 1 : 1,
            Nome = nome.Trim(),
            Email = email.Trim(),
            Cargo = cargo.Trim(),
            Ativo = ativo,
            DataCadastro = DateTime.UtcNow
        };

        _usuarios.Add(usuario);
        return usuario;
    }

    public Usuario? Atualizar(int id, string nome, string email, string cargo, bool ativo)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario is null)
        {
            return null;
        }

        usuario.Nome = nome.Trim();
        usuario.Email = email.Trim();
        usuario.Cargo = cargo.Trim();
        usuario.Ativo = ativo;

        return usuario;
    }

    public bool Remover(int id)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);

        if (usuario is null)
        {
            return false;
        }

        _usuarios.Remove(usuario);
        return true;
    }

    public int ObterQuantidadeAtivos()
    {
        return _usuarios.Count(u => u.Ativo);
    }
}

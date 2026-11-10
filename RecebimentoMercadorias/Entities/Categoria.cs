using System.ComponentModel.DataAnnotations.Schema;

namespace RecebimentoMercadorias.Entities;

public sealed class Categoria
{
    [Column("id_categoria")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }
}using System.ComponentModel.DataAnnotations.Schema;

namespace RecebimentoMercadorias.Entities;

public sealed class Categoria
{
    [Column("id_categoria")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("data_cadastro")]
    public DateTime? DataCadastro { get; set; } = DateTime.UtcNow;
}
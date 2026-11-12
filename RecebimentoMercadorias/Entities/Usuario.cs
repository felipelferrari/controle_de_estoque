using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RecebimentoMercadorias.Entities;

[Table("usuarios")]
public sealed class Usuario
{
    [Column("id_usuario")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("cargo")]
    public string Cargo { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("data_cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    [Column("auth_user_id")]
    [JsonIgnore]
    public Guid? AuthUserId { get; set; }
}
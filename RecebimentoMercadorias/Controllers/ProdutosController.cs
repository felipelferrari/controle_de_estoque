using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecebimentoMercadorias.Data;
using RecebimentoMercadorias.Entities;

namespace RecebimentoMercadorias.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Get()
    {
        return await _context.Produtos
            .Include(p => p.Fornecedor)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Produto>> GetById(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null) return NotFound();

        return Ok(produto);
    }

    // Endpoint acionado ao bipar o leitor de código de barras
    [HttpGet("codigo-barras/{codigo}")]
    public async Task<ActionResult<Produto>> GetByCodigoBarras(string codigo)
    {
        var produto = await _context.Produtos
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.CodigoBarras == codigo.Trim());

        if (produto is null)
        {
            return NotFound("Produto não encontrado para o código de barras informado.");
        }

        return Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> Post([FromBody] CreateProdutoRequest request)
    {
        var codigoExistente = await _context.Produtos
            .AnyAsync(p => p.CodigoBarras == request.CodigoBarras.Trim());

        if (codigoExistente)
        {
            return BadRequest("Já existe um produto cadastrado com este código de barras.");
        }

        var produto = new Produto
        {
            CodigoBarras = request.CodigoBarras.Trim(),
            Nome = request.Nome.Trim(),
            PrecoCusto = request.PrecoCusto,
            PrecoVenda = request.PrecoVenda,
            QuantidadeEstoque = request.QuantidadeEstoque,
            EstoqueMinimo = request.EstoqueMinimo,
            DataValidade = request.DataValidade,
            FornecedorId = request.FornecedorId
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    // Entrada rápida de estoque acionada por BIP
    [HttpPost("entrada-bip")]
    public async Task<ActionResult> RegistraEntradaPorBip([FromBody] EntradaBipRequest request)
    {
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.CodigoBarras == request.CodigoBarras.Trim());

        if (produto is null) return NotFound("Produto não cadastrado.");

        produto.QuantidadeEstoque += request.Quantidade;

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produto.Id,
            Quantidade = request.Quantidade,
            Tipo = TipoMovimentacao.Entrada,
            UsuarioId = request.UsuarioId,
            DataMovimentacao = DateTime.UtcNow
        };

        _context.MovimentacoesEstoque.Add(movimentacao);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Estoque atualizado com sucesso.", NovoEstoque = produto.QuantidadeEstoque });
    }
}

public sealed class CreateProdutoRequest
{
    public string CodigoBarras { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int QuantidadeEstoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public DateTime? DataValidade { get; set; }
    public int FornecedorId { get; set; }
}

public sealed class EntradaBipRequest
{
    public string CodigoBarras { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 1;
    public int UsuarioId { get; set; }
}
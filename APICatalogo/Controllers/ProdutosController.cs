using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    // /api/produtos/primeiro
    [HttpGet("primeiro")]
    [HttpGet("teste")]
    [HttpGet("/primeiro")]
    public ActionResult<Produto> GetPrimeiro()
    {
        var produto = _context.Produtos.AsNoTracking().FirstOrDefault();

        if (produto is null) return NotFound("Produtos não encontrados...");

        return produto;
    }

    [HttpGet("{valor:alpha:length(5)}")]
    public ActionResult<Produto> Get2(string valor)
    {
        var teste = valor;

        var produto = _context.Produtos.AsNoTracking().FirstOrDefault();

        return produto;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _context.Produtos.AsNoTracking().Take(10).ToList();

        if (produtos is null)
        {
            return NotFound("Produtos não encontrados...");
        }

        return produtos;
    }

    [HttpGet("{id:int:min(1)}/{nome=Caderno}", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id, string nome)
    {
        var parametro = nome;

        var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        return produto;
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if (produto is null) { return BadRequest(); }

        _context.Produtos.Add(produto);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId) return BadRequest();

        _context.Entry(produto).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

        if (produto is null) return NotFound("Produto não localizado...");

        _context.Produtos.Remove(produto);
        _context.SaveChanges();

        return Ok(produto);
    }

}

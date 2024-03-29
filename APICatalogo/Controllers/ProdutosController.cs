using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;

    public ProdutosController(IUnitOfWork uof)
    {
        _uof = uof;
    }

    [HttpGet("produtos/{id}")]
    public ActionResult <IEnumerable<Produto>> GetProdutosPorCategoria(int id)
    {
        var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);

        if (produtos is null)
            return NotFound();

        return Ok(produtos);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _uof.ProdutoRepository.GetAll();

        if (produtos is null)
        {
            return NotFound("Produtos não encontrados...");
        }

        return Ok(produtos);
    }

    //[HttpGet("{id:int:min(1)}/{nome=Caderno}", Name = "ObterProduto")]
    [HttpGet("{id}", Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id, [BindRequired] string nome)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }

        return Ok(produto);
    }

    [HttpPost]
    public ActionResult Post(Produto produto)
    {
        if (produto is null) { return BadRequest(); }

        var novoProduto = _uof.ProdutoRepository.Create(produto);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if (id != produto.ProdutoId) return BadRequest();

        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
        _uof.Commit();

        return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

        if (produto is null)
            return NotFound("Produto não encontrado...");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        _uof.Commit();
        return Ok(produtoDeletado);
    }

    #region Alguns testes

    //[HttpGet("action")]
    //public ActionResult<Produto> Get4()
    //{
    //    var produto = _context.Produtos.AsNoTracking().FirstOrDefault();

    //    if (produto is null) return NotFound("Produto não encontrados...");

    //    return produto;
    //}

    //[HttpGet("iaction")]
    //public IActionResult Get3()
    //{
    //    var produto = _context.Produtos.AsNoTracking().Take(10).ToList();

    //    if (produto is null) return NotFound("Produto não encontrados...");

    //    return Ok(produto);
    //}

    //// /api/produtos/primeiro
    //[HttpGet("primeiro")]
    //[HttpGet("teste")]
    //[HttpGet("/primeiro")]
    //public ActionResult<Produto> GetPrimeiro()
    //{
    //    var produto = _context.Produtos.AsNoTracking().FirstOrDefault();

    //    if (produto is null) return NotFound("Produtos não encontrados...");

    //    return produto;
    //}

    //[HttpGet("{valor:alpha:length(5)}")]
    //public ActionResult<Produto> Get2(string valor)
    //{
    //    var teste = valor;

    //    var produto = _context.Produtos.AsNoTracking().FirstOrDefault();

    //    return produto;
    //}

    #endregion Alguns testes

}

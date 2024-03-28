using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IRepository<Categoria> _repository;
    private readonly ILogger _logger;
    private readonly IConfiguration _configurantion;

    public CategoriasController(
        IRepository<Categoria> repository, 
        IConfiguration configurantion, 
        ILogger<CategoriasController> logger)
    {
        _repository = repository;
        _logger = logger;
        _configurantion = configurantion;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        //_logger.LogInformation("=================GET api/categorias=================");
        var categorias = _repository.GetAll();
        return Ok(categorias);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id) 
    {
        var categoria = _repository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"=================GET api/categorias/id = {id} NOT FOUND=================");
            return NotFound($"Categoria com id={id} não encontrada...");
        }

        return Ok(categoria);
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        if (categoria is null)
        {
            _logger.LogWarning("Dados inválidos...");
            return BadRequest("Dados inválidos");
        }

        var categoriaCriada = _repository.Create(categoria);

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos");
            return BadRequest("Dados inválidos");
        }

        _repository.Update(categoria);
        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<Categoria> Delete(int id) 
    {
        var categoria = _repository.Get(c => c.CategoriaId == id);

        if (categoria is null) return NotFound($"Categoria com id={id} não encontrada...");

        var categoriaExcluida = _repository.Delete(categoria);
        return Ok(categoriaExcluida);
    }

    #region Alguns testes

    [HttpGet("LerArquivoConfiguracao")]
    public string GetValores()
    {
        var valor1 = _configurantion["chave1"];
        var valor2 = _configurantion["chave2"];

        var secao1 = _configurantion["secao1:chave2"];

        return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
    }

    [HttpGet("SemUsarFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoSemFromServices(IMeuServico meuServico, string nome)
    {
        return meuServico.Saudacao(nome);
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        throw new NotImplementedException();
        //_logger.LogInformation("=================GET api/categorias/produtos=================");

        //return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
        //return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).AsNoTracking().ToList();
    }

    #endregion Alguns testes

}

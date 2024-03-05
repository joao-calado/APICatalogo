using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configurantion;

    public CategoriasController(AppDbContext context, IConfiguration configurantion)
    {
        _context = context;
        _configurantion = configurantion;
    }

    [HttpGet("LerArquivoConfiguracao")]
    public string GetValores()
    {
        var valor1 = _configurantion["chave1"];
        var valor2 = _configurantion["chave2"];

        var secao1 = _configurantion["secao1:chave2"];

        return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
    }

    [HttpGet("UsandoFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico meuServico, string nome)
    {
        return meuServico.Saudacao(nome);
    }

    [HttpGet("SemUsarFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoSemFromServices(IMeuServico meuServico, string nome)
    {
        return meuServico.Saudacao(nome);
    }

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        //return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
        return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).AsNoTracking().ToList();
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))] 
    public ActionResult<IEnumerable<Categoria>> Get()
    {
        try
        {
            //throw new DataMisalignedException();
            return _context.Categorias.AsNoTracking().ToList();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<Categoria> Get(int id) 
    {
        try
        {
            var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);

            if (categoria == null) return NotFound($"Categoria com id={id} não encontrada...");

            return Ok(categoria);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    [HttpGet("{id:int:min(1)}/catNome=Adulto", Name = "ObterCategoria")]
    public ActionResult<Categoria> GetComtratamentoExcecao(int id)
    {
        //throw new Exception("Exceção ao retornar a categoria pelo Id");
        string[] teste = null;
        if (teste.Length > 0) { }

        var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);

        if (categoria == null) return NotFound($"Categoria com id={id} não encontrada...");

        return Ok(categoria);
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
        if (categoria is null) return BadRequest("Dados inválidos");

        _context.Categorias.Add(categoria);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
        if (id != categoria.CategoriaId) return BadRequest("Dados inválidos");

        _context.Entry(categoria).State = EntityState.Modified;
        _context.SaveChanges();
        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<Categoria> Delete(int id) 
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

        if (categoria is null) return NotFound($"Categoria com id={id} não encontrada...");

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        return Ok(categoria);
    }

}

﻿using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
// Inibe a exibição da coumentação para o endpoint
//[ApiExplorerSettings(IgnoreApi = true)]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet("produtos/{id}")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int id)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

        if (produtos is null)
            return NotFound();

        // var destino = _mapper.Map<Destino>(origem);
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }
    
    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);
        return ObterProdutos(produtos);
    }

    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFiltroPreco([FromQuery] ProdutosFiltroPreco produtosFilterParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFilterParameters);
        return ObterProdutos(produtos);
    }

    /// <summary>
    /// Exibe uma relação dos produtos
    /// </summary>
    /// <returns>Retorna uma lista de objetos Produto</returns>
    [Authorize(Policy = "UserOnly")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
    {
        try
        {
            var produtos = await _uof.ProdutoRepository.GetAllAsync();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtosDto);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Obtem o produto pelo seu identificador produtoId
    /// </summary>
    /// <param name="id">Código do produto</param>
    /// <returns>Um objeto produto</returns>
    //[HttpGet("{id:int:min(1)}/{nome=Caderno}", Name = "ObterProduto")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int? id)
    {
        if (id == null || id <= 0)
        {
            return BadRequest("ID do produto inválido");
        }

        var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto não encontrado...");
        }
        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null) { return BadRequest(); }

        var produto = _mapper.Map<Produto>(produtoDto);

        var novoProduto = _uof.ProdutoRepository.Create(produto);
        await _uof.CommitAsync();

        var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

        return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
    }

    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id,
        JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        //Valores possíveis: replace, add, remove, copy, move e test

        if (patchProdutoDTO is null || id <= 0)
            return BadRequest();

        var produto = await _uof.ProdutoRepository.GetAsync(c => c.ProdutoId == id);

        if (produto is null)
            return BadRequest();

        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(produtoUpdateRequest, produto);

        _uof.ProdutoRepository.Update(produto);
        await _uof.CommitAsync();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId) return BadRequest();//400

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
        await _uof.CommitAsync();

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.ProdutoId == id);
        if (produto is null)
            return NotFound("Produto não encontrado...");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        await _uof.CommitAsync();

        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

        return Ok(produtoDeletadoDto);
    }

    #region Métodos privados

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(IPagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.Count,
            produtos.PageSize,
            produtos.PageCount,
            produtos.TotalItemCount,
            produtos.HasNextPage,
            produtos.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    #endregion Métodos privados

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

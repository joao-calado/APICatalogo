using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests.UnitTests;

public class PutProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public PutProdutoUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    // Testes de unidade para PUT
    [Fact]
    public async Task PutProduto_Return_OkResult()
    {
        // Arrange
        var prodId = 13;

        var updateProdutoDto = new ProdutoDTO
        {
            ProdutoId = prodId,
            Nome = "Novo Produto (xUnit)",
            Descricao = "Descrição do Novo Produto (xUnit) - att",
            Preco = 10.99m,
            ImagemUrl = "imagemFake1.jpg",
            CategoriaId = 2
        };

        // Act
        var result = await _controller.Put(prodId, updateProdutoDto) as ActionResult<ProdutoDTO>;

        // Assert
        result.Should().NotBeNull(); // Verifica se o resultado não é nulo
        result.Result.Should().BeOfType<OkObjectResult>(); // Verifica se o resultado é OkObjectResult
    }

    [Fact]
    public async Task Produto_Return_BadRequest()
    {
        // Arrange
        var prodId = 1000;

        var updateProdutoDto = new ProdutoDTO
        {
            ProdutoId = 13,
            Nome = "Novo Produto (xUnit)",
            Descricao = "Descrição do Novo Produto (xUnit)",
            ImagemUrl = "imagemFake1.jpg",
            CategoriaId = 2
        };

        // Act
        var data = await _controller.Put(prodId, updateProdutoDto);

        // Assert
        data.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be(400);
    }
}

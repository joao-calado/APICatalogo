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

public class PostProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public PostProdutoUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    // Métodos de testes para POST
    [Fact]
    public async Task PostProduto_Return_CreatedStatusCode()
    {
        // Arrange
        var novoProdutoDto = new ProdutoDTO
        {
            Nome = "Novo Produto (xUnit)",
            Descricao = "Descrição do Novo Produto (xUnit)",
            Preco = 10.99m,
            ImagemUrl = "imagemFake1.jpg",
            CategoriaId = 2
        };

        // Act
        var data = await _controller.Post(novoProdutoDto);

        // Assert
        var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
        createdResult.Subject.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task PostProduto_Return_BadRequest()
    {
        // Arrange
        ProdutoDTO prod = null;

        // Act
        var data = await _controller.Post(prod);

        // Assert
        var badRequestResult = data.Result.Should().BeOfType<BadRequestResult>();
        badRequestResult.Subject.StatusCode.Should().Be(400);
    }
}

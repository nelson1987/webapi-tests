using Managemt.Api.Features.Users;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Management.Domain.Tests;

[Trait("Category", "UnitTests")]
public class PagamentoControllerTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly PagamentoController _controller;

    public PagamentoControllerTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _controller = new PagamentoController(null, null, _tokenServiceMock.Object);
    }

    [Fact]
    public async void GetToken_ReturnsOk_WithValidUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "Batman", Password = "Batman", Role = Settings.AuthorizationTypes.Employee };
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("valid_token");

        // Act
        var result = await _controller.GetToken(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("valid_token", okResult.Value);
    }

    [Fact]
    public async void GetToken_ReturnsUnauthorized_WithInvalidUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "Batman", Password = "invalid_password", Role = Settings.AuthorizationTypes.Employee };
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("valid_token");

        // Act
        var result = await _controller.GetToken(CancellationToken.None);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async void GetToken_ReturnsBadRequest_WhenUserIsNull()
    {
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("valid_token");

        // Act
        var result = await _controller.GetToken(CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async void GetToken_ReturnsBadRequest_WhenTokenServiceThrowsException()
    {
        // Arrange
        var user = new User { Id = 1, Username = "Batman", Password = "Batman", Role = Settings.AuthorizationTypes.Employee };
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Throws<Exception>();

        // Act
        var result = await _controller.GetToken(CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}

//[Trait("Category", "UnitTests")]
//public class PagamentoUnitTest
//{
//    [Fact]
//    public void Pagamento()
//    {
//    }
//}

//[Trait("Category", "UnitTests")]
//public class PagamentoEventUnitTest
//{
//    [Fact]
//    public void Pagamento()
//    {
//    }
//}

//public record PagamentoEvent(int Id, string NumeroContrato, decimal Preco, int Quantidade, int IdEstoque);

//[Trait("Category", "UnitTests")]
//public class PagamentoConsumerUnitTest
//{
//    [Fact]
//    public void Test1()
//    {
//    }
//}

//public class PagamentoConsumer
//{
//    public async Task<Pagamento> Inserir(CancellationToken cancellationToken = default)
//    {
//        return await Task.FromResult(new Pagamento());
//    }
//}

//[Trait("Category", "UnitTests")]
//public class PagamentoPublisherUnitTest
//{
//    [Fact]
//    public void Test1()
//    {
//    }
//}

//public class PagamentoPublisher
//{
//    public async Task<Pagamento> Inserir(CancellationToken cancellationToken = default)
//    {
//        return await Task.FromResult(new Pagamento());
//    }
//}
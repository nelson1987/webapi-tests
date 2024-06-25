using System.Net;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentAssertions;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Management.Domain.Tests;

[Trait("Category", "UnitTests")]
public class PagamentoControllerUnitTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly PagamentoController _sut;
    private readonly PagamentoCommand _command;
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<IPagamentoHandler> _mockHandler;
    private readonly Mock<IValidator<PagamentoCommand>> _mockValidator;

    public PagamentoControllerUnitTest()
    {
        _command = _fixture.Create<PagamentoCommand>();

        _mockHandler = _fixture.Freeze<Mock<IPagamentoHandler>>();
        _mockValidator = _fixture.Freeze<Mock<IValidator<PagamentoCommand>>>();

        _fixture.Freeze<Mock<IPagamentoHandler>>()
                .Setup(x => x.HandleAsync(_command, _token))
                .ReturnsAsync(new Pagamento(_command.NumeroContrato, _command.Preco, _command.Quantidade));

        _fixture.Freeze<Mock<IValidator<PagamentoCommand>>>()
                .Setup(x => x.ValidateAsync(_command, _token))
                .ReturnsAsync(new ValidationResult());

        _sut = _fixture.Build<PagamentoController>()
            .OmitAutoProperties()
            .Create();
    }

    [Fact]
    public async Task Dado_Commando_Valido_Retorna_Created()
    {
        var result = await _sut.Post(_command, _token);
        var createdResult = (ObjectResult)result;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        _mockHandler.Verify(x => x.HandleAsync(_command, _token), Times.Once);
    }

    [Fact]
    public async Task Dado_Commando_Invalido_Retorna_UnprocessableEntity()
    {
        _fixture.Freeze<Mock<IValidator<PagamentoCommand>>>()
                .Setup(x => x.ValidateAsync(_command, _token))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("any-prop", "any-error-message") }));

        var result = await _sut.Post(_command, _token);
        var createdResult = (BadRequestObjectResult)result;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockHandler.Verify(x => x.HandleAsync(_command, _token), Times.Never);
    }

    [Fact]
    public async Task Dado_Exception_Retorna_BadRequest()
    {
        _fixture.Freeze<Mock<IPagamentoHandler>>()
                .Setup(x => x.HandleAsync(It.IsAny<PagamentoCommand>(), _token))
                .ThrowsAsync(new NotImplementedException("whatever"));
        var result = await _sut.Post(_command, _token);
        var createdResult = (BadRequestResult)result;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockHandler.Verify(x => x.HandleAsync(It.IsAny<PagamentoCommand>(), _token), Times.Once);
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
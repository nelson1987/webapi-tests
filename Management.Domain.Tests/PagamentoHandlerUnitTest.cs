using AutoFixture;
using AutoFixture.AutoMoq;

using FluentAssertions;

using FluentValidation;

using Moq;

namespace Management.Domain.Tests;

[Trait("Category", "UnitTests")]
public class PagamentoHandlerUnitTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly PagamentoHandler _sut;
    private readonly PagamentoCommand _command;
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<IPagamentoRepository> _repositoryMock;

    public PagamentoHandlerUnitTest()
    {
        _repositoryMock = _fixture.Freeze<Mock<IPagamentoRepository>>();
        _command = _fixture.Create<PagamentoCommand>();
        _sut = _fixture.Create<PagamentoHandler>();
    }

    [Fact]
    public async void Dado_Comando_Inserir_Corretamente_Retorna_Pagamento_Inserido()
    {
        var result = await _sut.HandleAsync(_command, _token);
        result.Should().NotBeNull();
        result.Id.Should().NotBe(0);
        result.NumeroContrato.Should().NotBe(_command.NumeroContrato);
        _repositoryMock.Verify(x => x.Inserir(It.IsAny<Pagamento>(), _token), Times.Once);
    }

    [Fact]
    public async void Dado_Comando_Inserir_Duplicidade_Retorna_Pagamento_Inserido()
    {
        _fixture.Freeze<Mock<IPagamentoRepository>>()
                .Setup(x => x.Inserir(It.IsAny<Pagamento>(), _token))
                .ThrowsAsync(new NotImplementedException("whatever"));
        Func<Task> result = async () => await _sut.HandleAsync(_command, _token);
        await result.Should().ThrowAsync<NotImplementedException>()
            .WithMessage("whatever");
        _repositoryMock.Verify(x => x.Inserir(It.IsAny<Pagamento>(), _token), Times.Once);
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
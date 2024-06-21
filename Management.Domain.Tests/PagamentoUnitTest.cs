using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Management.Domain.Tests;

//[Trait("Category", "UnitTests")]
//public class PagamentoCommandUnitTest
//{
//    [Fact]
//    public void Pagamento()
//    {
//    }
//}
[Trait("Category", "UnitTests")]
public class PagamentoMappingTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly PagamentoCommand _command;
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _config;

    public PagamentoMappingTests()
    {
        _config = new MapperConfiguration(cfg =>
            cfg.AddProfile(typeof(PagamentoMapping))
        );
        _mapper = _config.CreateMapper();
        _command = _fixture.Create<PagamentoCommand>();
    }

    [Fact]
    public void ConfigurationTest()
    {
        _config.AssertConfigurationIsValid();
    }

    [Fact]
    public void CommandToEntityTest()
    {
        var destination = _mapper.Map<Pagamento>(_command);
        destination.Should().NotBeNull();

        destination.Id.Should().Be(0);
        destination.IdEstoque.Should().Be(0);
        destination.NumeroContrato.Should().Be(_command.NumeroContrato);
        destination.Preco.Should().Be(_command.Preco);
        destination.Quantidade.Should().Be(_command.Quantidade);
    }
}

[Trait("Category", "UnitTests")]
public class PagamentoRepositoryUnitTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly PagamentoRepository _sut;
    private readonly Pagamento _entity;
    private readonly CancellationToken _token = CancellationToken.None;

    public PagamentoRepositoryUnitTest()
    {
        _entity = _fixture.Build<Pagamento>()
            .With(x => x.Id, 0)
            .Create();
        _sut = _fixture.Create<PagamentoRepository>();
    }

    [Fact]
    public async Task Insercao_Valida_Retorna_Entitidade_Criada()
    {
        var result = await _sut.Inserir(_entity, _token);
        result.Should().BeSameAs(_entity);
    }

    [Fact]
    public async Task Insercao_Invalida_Se_Id_Diferente_Zero()
    {
        _entity.Id = 1;
        Func<Task> result = async () => await _sut.Inserir(_entity, _token);
        await result.Should().ThrowAsync<NotImplementedException>()
            .WithMessage("whatever");
    }
}

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
        var result = await _sut.Handler(_command, _token);
        result.Should().NotBeNull();
        result.Id.Should().NotBe(0);
        result.NumeroContrato.Should().NotBe(_command.NumeroContrato);
    }

    [Fact]
    public async void Dado_Comando_Inserir_Duplicidade_Retorna_Pagamento_Inserido()
    {
        _fixture.Freeze<Mock<IPagamentoRepository>>()
                .Setup(x => x.Inserir(It.IsAny<Pagamento>(), _token))
                .ThrowsAsync(new NotImplementedException("whatever"));
        Func<Task> result = async () => await _sut.Handler(_command, _token);
        await result.Should().ThrowAsync<NotImplementedException>()
            .WithMessage("whatever");
    }
}

[Trait("Category", "UnitTests")]
public class PagamentoControllerUnitTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly PagamentoController _sut;
    private readonly PagamentoCommand _command;
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<IPagamentoHandler> _mockHandler;

    public PagamentoControllerUnitTest()
    {
        _mockHandler = _fixture.Freeze<Mock<IPagamentoHandler>>();
        _fixture.Freeze<Mock<IPagamentoHandler>>()
                .Setup(x => x.Handler(It.IsAny<PagamentoCommand>(), _token))
                .ReturnsAsync(new Pagamento());

        _command = _fixture.Create<PagamentoCommand>();
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
    }

    [Fact]
    public async Task Dado_Exception_Retorna_Created()
    {
        _fixture.Freeze<Mock<IPagamentoHandler>>()
                .Setup(x => x.Handler(It.IsAny<PagamentoCommand>(), _token))
                .ThrowsAsync(new NotImplementedException("whatever"));
        var result = await _sut.Post(_command, _token);
        var createdResult = (BadRequestResult)result;
        createdResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
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
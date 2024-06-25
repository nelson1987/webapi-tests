using AutoFixture;
using AutoFixture.AutoMoq;

using AutoMapper;

using FluentAssertions;

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
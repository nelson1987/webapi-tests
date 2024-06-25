using AutoFixture;
using AutoFixture.AutoMoq;

using FluentAssertions;

using FluentValidation;

namespace Management.Domain.Tests;

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

    [Fact]
    public async Task Insercao_Invalida_Se_Entidade_Existir()
    {
        var result = await _sut.Inserir(_entity, _token);
        result.Should().BeSameAs(_entity);
    }

    //TODO: Duplicado

    //_repository.AddAsync(Arg.Any<Contributor>(), Arg.Any<CancellationToken>())
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
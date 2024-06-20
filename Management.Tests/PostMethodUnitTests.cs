using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
namespace Management.Tests;

[Trait("Category", "UnitTests")]
public class UnitOfWorkUnitTests
{
}

[Trait("Category", "UnitTests")]
public class FinancingRepositorioUnitTests
{
}

[Trait("Category", "UnitTests")]
public class AccountRepositorioUnitTests
{
}

[Trait("Category", "UnitTests")]
public class MessageBrokerUnitTests
{
}

[Trait("Category", "UnitTests")]
public class PostMethodUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<IHandler> _servico;

    public PostMethodUnitTests()
    {
        //Arrange
        _servico = _fixture.Freeze<Mock<IHandler>>();
        _fixture.Freeze<Mock<IHandler>>()
                .Setup(x => x.Inserir(It.IsAny<Financing>()));
    }

    [Fact]
    public async Task Post_Method_should_Return_Created()
    {
        //Arrange
        var contractNumber = Guid.NewGuid().ToString();
        var financing = _fixture.Build<Financing>()
                                .With(x => x.ContractNumber, contractNumber)
                                .Create();
        //Act
        var result = await EmployeeEndpoint.CreateTodo(_servico.Object, financing);
        //Assert
        var createdResult = (Created)result.Result;
        createdResult.StatusCode.Should().Be((int)System.Net.HttpStatusCode.Created);

        _fixture.Freeze<Mock<IHandler>>()
            .Verify(x => x.Inserir(It.IsAny<Financing>()), Times.Once());
        //_fixture.Freeze<Mock<IRepositorio>>()
        //    .Verify(x => x.BuscarNumero(2, _token), Times.Once());
        //_fixture.Freeze<Mock<IRepositorio>>()
        //    .Verify(x => x.BuscarNumero(3, _token), Times.Never());
    }

    //[Fact]
    //public async Task Post_Method_Has_Servico_Without_Data_Should_Return_BadRequest()
    //{
    //    //Arrange
    //    //_fixture.Freeze<Mock<IServico>>()
    //    //        .Setup(x => x.Inserir(It.IsAny<Financing>()))
    //    //        .Throws(new Exception());
    //
    //    var contractNumber = Guid.NewGuid().ToString();
    //    var financing = _fixture.Build<Financing>()
    //                            .With(x => x.ContractNumber, contractNumber)
    //                            .Create();
    //    //Act
    //    var result = await EmployeeEndpoint.CreateTodo(_servico.Object, financing);
    //    //Assert
    //    var createdResult = (BadRequest)result.Result;
    //    createdResult.StatusCode.Should().Be((int)System.Net.HttpStatusCode.BadRequest);
    //    //result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    //}
    //Buscar da base Por Numero do contrato
    //Validar de existe
    //Se sim: retorna invalidez
    //Se não: tentar salvar na base
    //Enviar para message Broker


    //unitOfWork.BeginTransaction - Erro
    //financingRepositorio.Inserir - Erro
    //accountRepositorio.Inserir - Erro
    //unitOfWork.Commit - Erro
    //unitOfWork.Rollback - Erro
}
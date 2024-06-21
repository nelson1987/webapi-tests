using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Managemt.Api.Core.Domain.Entities;
using MassTransit;
using MassTransit.Testing;
using Moq;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Management.Tests;

[Trait("Category", "UnitTests")]
public class FinancingConsumerTests : IAsyncDisposable
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly InMemoryTestHarness _harness;
    private readonly ConsumerTestHarness<FinancingConsumer> _sut;
    private readonly FinanciamentoEvent _event;
    private string _numeroContrato => Guid.NewGuid().ToString();

    public FinancingConsumerTests()
    {
        _harness = new InMemoryTestHarness();
        _sut = _harness.Consumer<FinancingConsumer>();
        _harness.Start();
        _event = _fixture.Build<FinanciamentoEvent>()
            .With(x => x.NumeroContrato == _numeroContrato)
            .Create();
    }

    public async ValueTask DisposeAsync()
    {
        await _harness.Stop();
    }

    [Fact]
    public async Task Should_Process_Message()
    {
        await _harness.Bus.Publish(_event);
        _sut.Consumed.Select<FinanciamentoEvent>().Any().Should().BeTrue();
    }

    [Fact]
    public async Task Should_Consume_Message_Pub_Sub()
    {
        var numeroContratoExpected = "QualquerNumero";
        var message = _event with { NumeroContrato = numeroContratoExpected };
        await _harness.Bus.Publish(message);
        _harness.Published.Select<FinanciamentoEvent>().Count().Should().Be(1);
        var consumedInvoiceCreated = _harness.Published.Select<FinanciamentoEvent>()
                .First().Context.Message;
        consumedInvoiceCreated.NumeroContrato.Should().Be(numeroContratoExpected);
    }
}

[Trait("Category", "UnitTests")]
public class FinancingCommandHandlerUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly Mock<IProdutoRepository> _repositorio;
    private readonly Mock<IRiskClienteExterno> _riskClient;
    private readonly Mock<IValidator<FinanciamentoCommand>> _validator;
    private readonly FinancingCommandHandler _sut;
    private readonly FinanciamentoCommand _command;
    private readonly CancellationToken _token = CancellationToken.None;

    public FinancingCommandHandlerUnitTests()
    {
        _repositorio = _fixture.Freeze<Mock<IProdutoRepository>>();
        _validator = _fixture.Freeze<Mock<IValidator<FinanciamentoCommand>>>();
        _riskClient = _fixture.Freeze<Mock<IRiskClienteExterno>>();

        _fixture.Freeze<Mock<IProdutoRepository>>()
                .Setup(x => x.Find(It.IsAny<Func<Financiamento, bool>>(), _token))
                .ReturnsAsync(It.IsAny<Financiamento>());

        _fixture.Freeze<Mock<IValidator<FinanciamentoCommand>>>()
                .Setup(x => x.ValidateAsync(It.IsAny<FinanciamentoCommand>(), _token))
                .ReturnsAsync(new ValidationResult());

        _fixture.Freeze<Mock<IRiskClienteExterno>>()
                .Setup(x => x.ValidateAsync(_token))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                });

        _command = _fixture.Create<FinanciamentoCommand>();
        _sut = _fixture.Create<FinancingCommandHandler>();
    }

    [Fact]
    public async Task VerificarSeFinanciamentoExistente()
    {
        //Arrange
        _fixture.Freeze<Mock<IProdutoRepository>>()
                .Setup(x => x.Find(It.IsAny<Func<Financiamento, bool>>(), _token))
                .ReturnsAsync((Financiamento?)null);
        //Act
        var entity = await _sut.Handle(_command, _token);
        //Arrange
        entity.Should().NotBeNull();
    }

    [Fact]
    public async Task VerificarSeFinanciamentoInexistente()
    {
        //Act
        var result = await _sut.Handle(_command, _token);
        //Arrange
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task VerificarSeFinanciamentoValidandoEmRisco()
    {
        //Arrange
        var contractNumber = Guid.NewGuid().ToString();
        _fixture.Freeze<Mock<IRiskClienteExterno>>()
                .Setup(x => x.ValidateAsync(_token))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
        //Act
        var entity = await _sut.Handle(_command, _token);
        //Arrange
        entity.Should().NotBeNull();
    }

    [Fact]
    public async Task VerificarSeRequestValido()
    {
        //Arrange
        var contractNumber = Guid.NewGuid().ToString();

        _fixture.Freeze<Mock<IValidator<FinanciamentoCommand>>>()
                .Setup(x => x.ValidateAsync(It.IsAny<FinanciamentoCommand>(), _token))
                .ReturnsAsync(new ValidationResult(_fixture.CreateMany<ValidationFailure>(3)));
        //Act
        var entity = await _sut.Handle(_command, _token);
        //Arrange
        entity.Should().NotBeNull();
    }
}

[Trait("Category", "UnitTests")]
public class FinancingRepositoryUnitTests
{
    private readonly ProdutoRepository _sut;
    private readonly CancellationToken _token = CancellationToken.None;

    public FinancingRepositoryUnitTests()
    {
        _sut = new ProdutoRepository();
    }

    [Fact]
    public void ListarTodasEntidades()
    {
        //Arrange
        var contractNumber = Guid.NewGuid().ToString();
        //Act
        var listEntities = _sut.Find(x => x.ContractNumber == contractNumber, _token);
        //Arrange
        listEntities.Should().NotBeNull();
    }
}

[Trait("Category", "UnitTests")]
public class CustomerValidatorUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly FinanciamentoCommand request;
    private readonly FinancingValidator validator;

    public CustomerValidatorUnitTests()
    {
        validator = new FinancingValidator();
        request = _fixture.Build<FinanciamentoCommand>()
                                .Create();
    }

    [Fact]
    public void Financing_Invalido_Sem_Numero_Contrato()
        => validator.TestValidate(request with { NumeroContrato = string.Empty })
            .ShouldHaveValidationErrorFor(person => person.NumeroContrato);
}

/*
[Trait("Category", "UnitTests")]
public class PostMethodUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<IHandler> _sut;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IRiskClienteExterno> _riskClienteExterno;
    private readonly Mock<IFinancingRepositorio> _financingRepositorio;
    private readonly Mock<IAccountRepositorio> _accountRepositorio;
    private readonly Mock<IMessageBroker> _messageBroker;

    public PostMethodUnitTests()
    {
        //Arrange
        _sut = _fixture.Freeze<Mock<IHandler>>();
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
        //var result = await EmployeeEndpoint.CreateTodo(_sut.Object, financing);
        //Assert
        //var createdResult = (Created)result.Result;
        //createdResult.StatusCode.Should().Be((int)System.Net.HttpStatusCode.Created);

        //_fixture.Freeze<Mock<IHandler>>()
        //    .Verify(x => x.Inserir(It.IsAny<Financing>()), Times.Once());
        //_fixture.Freeze<Mock<IRepositorio>>()
        //    .Verify(x => x.BuscarNumero(2, _token), Times.Once());
        //_fixture.Freeze<Mock<IRepositorio>>()
        //    .Verify(x => x.BuscarNumero(3, _token), Times.Never());
    }
*/
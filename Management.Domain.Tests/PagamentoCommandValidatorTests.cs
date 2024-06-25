using AutoFixture;
using AutoFixture.AutoMoq;

using FluentValidation;
using FluentValidation.TestHelper;

namespace Management.Domain.Tests;

[Trait("Category", "UnitTests")]
public class PagamentoCommandValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly IValidator<PagamentoCommand> _validator;
    private readonly PagamentoCommand _command;

    public PagamentoCommandValidatorTests()
    {
        _command = _fixture.Build<PagamentoCommand>()
            .Create();
        _validator = _fixture.Create<PagamentoCommandValidator>();
    }

    [Fact]
    public void Dado_um_comando_valido_quando_todos_campos_foram_validos_deve_passar_validacao()
        => _validator
            .TestValidate(_command)
            .ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void Dado_um_comando_invalido_sem_numero_contrato_deve_falhar_validacao()
        => _validator
            .TestValidate(_command with { NumeroContrato = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.NumeroContrato)
            .Only();

    [Fact]
    public void Dado_um_comando_invalido_sem_preco_deve_falhar_validacao()
        => _validator
            .TestValidate(_command with { Preco = 0.00M })
            .ShouldHaveValidationErrorFor(x => x.Preco)
            .Only();

    [Fact]
    public void Dado_um_comando_invalido_sem_quantidade_deve_falhar_validacao()
        => _validator
            .TestValidate(_command with { Quantidade = 0 })
            .ShouldHaveValidationErrorFor(x => x.Quantidade)
            .Only();
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
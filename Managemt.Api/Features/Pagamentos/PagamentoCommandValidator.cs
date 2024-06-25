using FluentValidation;

namespace Managemt.Api.Features.Pagamentos;

public class PagamentoCommandValidator : AbstractValidator<PagamentoCommand>
{
    public PagamentoCommandValidator()
    {
        RuleFor(x => x.NumeroContrato).NotEmpty();
        RuleFor(x => x.Preco).GreaterThan(0);
        RuleFor(x => x.Quantidade).GreaterThan(0);
    }
}

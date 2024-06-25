namespace Managemt.Api.Features.Pagamentos;

public interface IPagamentoHandler
{
    Task<Pagamento> HandleAsync(PagamentoCommand command, CancellationToken cancellationToken = default);
}

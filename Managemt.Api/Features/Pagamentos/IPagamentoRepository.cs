namespace Managemt.Api.Features.Pagamentos;

public interface IPagamentoRepository
{
    Task<Pagamento> Inserir(Pagamento entity, CancellationToken cancellationToken = default);
}

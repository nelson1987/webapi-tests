namespace Managemt.Api.Features.Pagamentos;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly PagamentoContext _pagamentoContext;

    public PagamentoRepository(PagamentoContext pagamentoContext)
    {
        _pagamentoContext = pagamentoContext;
    }

    public async Task<Pagamento> Inserir(Pagamento entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id != 0) throw new NotImplementedException("whatever");
        entity.Id = 1;
        _pagamentoContext.AdicionarAsync(entity, cancellationToken);
        return await Task.FromResult(entity);
    }
}

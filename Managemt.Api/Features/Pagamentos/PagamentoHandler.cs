using Managemt.Api.Extensions;

namespace Managemt.Api.Features.Pagamentos;

public class PagamentoHandler : IPagamentoHandler
{
    private readonly IPagamentoRepository _pagamentoRepository;

    public PagamentoHandler(IPagamentoRepository pagamentoRepository)
    {
        _pagamentoRepository = pagamentoRepository;
    }

    public async Task<Pagamento> HandleAsync(PagamentoCommand command, CancellationToken cancellationToken = default)
    {
        Pagamento entity = command.MapTo<Pagamento>();
        var result = await _pagamentoRepository.Inserir(entity, cancellationToken);

        return await Task.FromResult(result);
    }
}

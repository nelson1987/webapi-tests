namespace Managemt.Api.Features.Pagamentos;

public class PagamentoContext
{
    public List<Pagamento> Listagem { get; private set; }

    public PagamentoContext()
    {
        Listagem = new List<Pagamento>();
    }

    public void AdicionarAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
    {
        Listagem.Add(pagamento);
    }
}

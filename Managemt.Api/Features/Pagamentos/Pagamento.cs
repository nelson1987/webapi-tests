using AutoMapper;
using System.Net;

namespace Managemt.Api.Features.Pagamentos;

public class PagamentoMapping : Profile
{
    public PagamentoMapping()
    {
        CreateMap<PagamentoCommand, Pagamento>();
    }
}

public class Pagamento
{
    public int Id { get; set; }
    public string? NumeroContrato { get; set; }
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
    public int IdEstoque { get; set; }
}

public record PagamentoCommand(string NumeroContrato, decimal Preco, int Quantidade);

public interface IPagamentoRepository
{
    Task<Pagamento> Inserir(Pagamento entity, CancellationToken cancellationToken = default);
}

public class PagamentoRepository : IPagamentoRepository
{
    public async Task<Pagamento> Inserir(Pagamento entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id != 0) throw new NotImplementedException("whatever");
        return await Task.FromResult(entity);
    }
}

public interface IPagamentoHandler
{
    Task<Pagamento> Handler(PagamentoCommand command, CancellationToken cancellationToken = default);
}

public class PagamentoHandler : IPagamentoHandler
{
    private readonly IPagamentoRepository _pagamentoRepository;

    public PagamentoHandler(IPagamentoRepository pagamentoRepository)
    {
        _pagamentoRepository = pagamentoRepository;
    }

    public async Task<Pagamento> Handler(PagamentoCommand command, CancellationToken cancellationToken = default)
    {
        //TODO: Pagamento entity = command.MapTo<Pagamento>();
        Pagamento entity = new Pagamento() { NumeroContrato = command.NumeroContrato, Preco = command.Preco, Quantidade = command.Quantidade };
        var result = await _pagamentoRepository.Inserir(entity, cancellationToken);
        return await Task.FromResult(result);
    }
}

public class PagamentoController
{
    private readonly IPagamentoHandler _pagamentoHandler;

    public PagamentoController(IPagamentoHandler pagamentoHandler)
    {
        _pagamentoHandler = pagamentoHandler;
    }

    public async Task<HttpStatusCode> Post(PagamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            Pagamento pagamento = await _pagamentoHandler.Handler(command, cancellationToken);
            return HttpStatusCode.Created;
        }
        catch (Exception)
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
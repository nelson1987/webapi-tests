using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Managemt.Api.Features.Pagamentos;

public static class PagamentoDependencies
{
    public static IServiceCollection AddPagamentoService(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(PagamentoMapping));
        services.AddScoped<IPagamentoHandler, PagamentoHandler>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        return services;
    }
}

public class PagamentoMapping : Profile
{
    public PagamentoMapping()
    {
        CreateMap<PagamentoCommand, Pagamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IdEstoque, opt => opt.Ignore());
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
        entity.Id = 1;
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
        Pagamento entity = command.MapTo<Pagamento>();
        var result = await _pagamentoRepository.Inserir(entity, cancellationToken);
        return await Task.FromResult(result);
    }
}

[ApiController]
[Consumes("application/json")]
[Route("/pagamentos")]
public class PagamentoController : ControllerBase
{
    private readonly IPagamentoHandler _pagamentoHandler;

    public PagamentoController(IPagamentoHandler pagamentoHandler)
    {
        _pagamentoHandler = pagamentoHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PagamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            Pagamento pagamento = await _pagamentoHandler.Handler(command, cancellationToken);
            return Created($"/{pagamento.Id}", null);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}
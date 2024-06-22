using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Managemt.Api.Features.Pagamentos;

public static class PagamentoDependencies
{
    public static IServiceCollection AddPagamentoService(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(PagamentoMapping));
        services.AddSingleton<PagamentoContext>();
        services.AddScoped<IPagamentoHandler, PagamentoHandler>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IValidator<PagamentoCommand>, PagamentoCommandValidator>();
        return services;
    }
}

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

public interface IPagamentoHandler
{
    Task<Pagamento> HandleAsync(PagamentoCommand command, CancellationToken cancellationToken = default);
}

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

public class PagamentoCommandValidator : AbstractValidator<PagamentoCommand>
{
    public PagamentoCommandValidator()
    {
        RuleFor(x => x.NumeroContrato).NotEmpty();
        RuleFor(x => x.Preco).GreaterThan(0);
        RuleFor(x => x.Quantidade).GreaterThan(0);
    }
}

[ApiController]
[Consumes("application/json")]
[Route("/pagamentos")]
public class PagamentoController : ControllerBase
{
    private readonly IPagamentoHandler _pagamentoHandler;
    private readonly IValidator<PagamentoCommand> _validator;

    public PagamentoController(IPagamentoHandler pagamentoHandler, IValidator<PagamentoCommand> validator)
    {
        _pagamentoHandler = pagamentoHandler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PagamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var validation = await _validator.ValidateAsync(command, cancellationToken);
            if (validation.IsInvalid())
                return UnprocessableEntity(validation.ToModelState());

            Pagamento pagamento = await _pagamentoHandler.HandleAsync(command, cancellationToken);
            return Created($"/{pagamento.Id}", null);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}

public static class FluentValidationExtensions
{
    public static bool IsInvalid(this FluentValidation.Results.ValidationResult result) => !result.IsValid;

    public static ModelStateDictionary ToModelState(this FluentValidation.Results.ValidationResult result)
    {
        var modelState = new ModelStateDictionary();
        result.Errors.ForEach(error =>
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        });
        return modelState;
    }
}
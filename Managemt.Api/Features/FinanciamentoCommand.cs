using FluentValidation;
using Managemt.Api.Core.Domain.Entities;
using MassTransit;
using System.Net;

namespace Managemt.Api.Features;

public record FinanciamentoEvent(int Id, string NumeroContrato, decimal Preco, int Quantidade, int IdEstoque);

public record FinanciamentoCommand(string NumeroContrato, decimal Preco, int Quantidade);

public interface IFinancingCommandHandler
{
    Task<(HttpStatusCode StatusCode, object Response)> Handle(FinanciamentoCommand financing, CancellationToken cancellationToken = default);
}

public interface IProdutoRepository : IBaseRepository<Financiamento>
{
}

public interface IRiskClienteExterno
{
    Task<HttpResponseMessage> ValidateAsync(CancellationToken cancellationToken = default);
}

public interface IBaseRepository<TEntity> : IDisposable where TEntity : class
{
    Task<TEntity> Insert(TEntity entity, CancellationToken cancellationToken = default);

    Task Update(TEntity entity, CancellationToken cancellationToken = default);

    Task Delete(TEntity entity, CancellationToken cancellationToken = default);

    Task<List<TEntity?>> FindAll();

    Task<List<TEntity?>> FindAll(Func<TEntity, bool> expression, CancellationToken cancellationToken = default);

    Task<TEntity?> Find(Func<TEntity, bool> expression, CancellationToken cancellationToken = default);

    Task<TEntity?> FindById(int id, CancellationToken cancellationToken = default);

    Task<int> Count(CancellationToken cancellationToken = default);

    Task<int> Count(Func<TEntity, bool> expression, CancellationToken cancellationToken = default);
}

public interface IEstoqueRepository : IBaseRepository<Estoque>
{
}

public interface IProdutoApplicationService
{
}

public class ProdutoApplicationService : IProdutoApplicationService
{
}

public class FinancingConsumer : IConsumer<FinanciamentoEvent>
{
    public async Task Consume(ConsumeContext<FinanciamentoEvent> context)
    {
        Console.WriteLine($"Processing message: {context.Message.NumeroContrato}");
    }
}

public static class ManagementApiConfig
{
    public static class Endpoints
    {
        public static string GET = "/{0}";
        public static string POST = "/";
    }
}

public class FinancingValidator : AbstractValidator<FinanciamentoCommand>
{
    public FinancingValidator()
    {
        RuleFor(x => x.NumeroContrato)
            .NotEmpty();
    }
}

public class FinancingCommandHandler : IFinancingCommandHandler
{
    private readonly ILogger<FinancingCommandHandler> _logger;
    private readonly IValidator<FinanciamentoCommand> _validator;
    private readonly IProdutoRepository _repository;
    private readonly IRiskClienteExterno _riskClienteExterno;
    private readonly IBus _bus;

    public FinancingCommandHandler(ILogger<FinancingCommandHandler> logger, IValidator<FinanciamentoCommand> validator, IProdutoRepository repository, IRiskClienteExterno riskClienteExterno, IBus bus)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
        _riskClienteExterno = riskClienteExterno;
        _bus = bus;
    }

    public async Task<(HttpStatusCode, object)> Handle(FinanciamentoCommand financing, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Mensagem a ser produzida {nameof(Handle)}.");
        try
        {
            var validationResult = await _validator.ValidateAsync(financing, cancellationToken);
            if (!validationResult.IsValid)
                return (HttpStatusCode.UnprocessableEntity, validationResult.Errors);

            var risk = await _riskClienteExterno.ValidateAsync(cancellationToken);
            if (!risk.IsSuccessStatusCode)
                return (HttpStatusCode.BadRequest, risk.Content);

            var entity = await _repository.Find(x => x.ContractNumber == financing.NumeroContrato, cancellationToken);
            if (entity == null)
                entity = await _repository.Insert(new Financiamento { ContractNumber = financing.NumeroContrato }, cancellationToken);

            try
            {
                await _bus.Publish(new FinanciamentoEvent(entity.Id, entity.ContractNumber!, entity.Preco, entity.Quantidade, entity.IdEstoque), cancellationToken);
            }
            catch (Exception ex)
            {
                return (HttpStatusCode.MultiStatus, ex.Message);
            }
            return (HttpStatusCode.Created, entity);
        }
        catch (Exception ex)
        {
            return (HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}

public class ProdutoRepository : IProdutoRepository
{
    private readonly List<Financiamento> _entities;

    public ProdutoRepository()
    {
        _entities = new List<Financiamento>();
    }

    public Task<int> Count(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> Count(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Financiamento t, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento?> Find(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_entities.FirstOrDefault(expression));
    }

    public Task<List<Financiamento?>> FindAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<Financiamento?>> FindAll(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento?> FindById(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento> Insert(Financiamento entity, CancellationToken cancellationToken = default)
    {
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public Task Update(Financiamento t, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class RiscoClienteHttp : IRiskClienteExterno
{
    public Task<HttpResponseMessage> ValidateAsync(CancellationToken cancellationToken = default)
    {
        //throw new NotImplementedException();
        return Task.FromResult(new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK
        });
    }
}
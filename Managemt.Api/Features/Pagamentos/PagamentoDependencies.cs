using FluentValidation;

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

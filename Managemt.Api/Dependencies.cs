using AutoMapper;
using Managemt.Api.Features.Pagamentos;

namespace Managemt.Api;

public static class Dependencies
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddPagamentoService();
        //builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
        //builder.Services.AddScoped<IFinancingCommandHandler, FinancingCommandHandler>();
        //builder.Services.AddScoped<IValidator<FinanciamentoCommand>, FinancingValidator>();
        //builder.Services.AddScoped<IRiskClienteExterno, RiscoClienteHttp>();
        //builder.Services.AddScoped<IConsumer<FinanciamentoEvent>, FinancingConsumer>();
        //builder.Services.AddMassTransit(x =>
        //{
        //    x.SetKebabCaseEndpointNameFormatter();
        //    x.AddConsumer<FinancingConsumer>();
        //    //x.UsingRabbitMq((ctx, cfg) =>
        //    //{
        //    //    //cfg.Host("localhost", "/", h =>
        //    //    //{
        //    //    //    h.Username("guest");
        //    //    //    h.Password("guest");
        //    //    //});
        //    //    cfg.Host("amqp://guest:guest@localhost:5672");
        //    //    cfg.ConfigureEndpoints(ctx);
        //    //    //cfg.UseRawJsonSerializer();
        //    //});
        //});
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();
        return builder;
    }

    public static WebApplication AddWebApplication(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.MapControllers();
        app.UseHttpsRedirection();
        return app;
    }
}

public static class Mappers
{
    private static readonly Lazy<IMapper> Lazy = new(() =>
    {
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Mappers).Assembly));
        return config.CreateMapper();
    });

    public static IMapper Mapper => Lazy.Value;

    public static T MapTo<T>(this object source) => Mapper.Map<T>(source);
}
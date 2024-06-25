using Managemt.Api.Features.Pagamentos;

namespace Managemt.Api.Services;

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
        builder.Services.AddControllers()
                        .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddProblemDetails();
        builder.Services.AddUserAuthentication();
        return builder;
    }

    public static WebApplication AddWebApplication(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseUserAuthentication();
        app.MapControllers();
        app.UseExceptionHandler();
        return app;
    }
}
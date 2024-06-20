using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IHandler, CreateFinancingHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
//builder.Services.AddAuthorizationBuilder()
//                .AddPolicy("admin_greetings", policy =>
//                    policy
//                        .RequireRole("admin")
//                        .RequireClaim("scope", "greetings_api"));
//builder.Services.AddAuthentication()
//                .AddJwtBearer();
//builder.Services.AddAuthorization();

WebApplication app = builder.Build();
app.UseExceptionHandler();
app.MapGroup("")//.MapGroup("/todos/v1")
    .MapEmployeeEndpoint()
    .WithTags("Todo Endpoints");
app.UseHttpsRedirection();
//app.UseCors();
//app.UseAuthentication();
//app.UseAuthorization();
app.Run();

public partial class Program { }

public static class EmployeeEndpoint
{
    public static RouteGroupBuilder MapEmployeeEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost(ManagementApiConfig.Endpoints.POST, CreateTodo);
        //.RequireAuthorization("admin_greetings");
        //.AddEndpointFilter(async (efiContext, next) =>
        //{
        //    var param = efiContext.GetArgument<TodoDto>(0);

        //    var validationErrors = Utilities.IsValid(param);

        //    if (validationErrors.Any())
        //    {
        //        return Results.ValidationProblem(validationErrors);
        //    }

        //    return await next(efiContext);
        //});
        return group;
    }
    public static async Task<Results<Created, BadRequest>> CreateTodo(IHandler servico, Financing financing)
    {
        servico.Inserir(financing);
        return TypedResults.Created();//$"/todos/v1/{financing.ContractNumber}", financing);
    }
}

public record Financing(string ContractNumber);

public static class ManagementApiConfig
{
    public static class Endpoints
    {
        public static string POST = "/weatherforecast";
    }
}

public interface IHandler
{
    void Inserir(Financing financing);
}

public class CreateFinancingHandler : IHandler
{
    UnitOfWork unitOfWork = new UnitOfWork();
    public void Inserir(Financing financing)
    {
        try
        {
            unitOfWork.BeginTransaction();
            FinancingRepositorio financingRepositorio = new FinancingRepositorio();
            financingRepositorio.Inserir();
            AccountRepositorio accountRepositorio = new AccountRepositorio();
            accountRepositorio.Inserir();
            MessageBroker broker = new MessageBroker();
            broker.Publish();
            unitOfWork.Commit();
        }
        catch (Exception)
        {
            unitOfWork.Rollback();
            throw;
        }
    }
}

public class UnitOfWork
{
    internal void BeginTransaction()
    {
        //throw new NotImplementedException();
    }

    internal void Commit()
    {
        //throw new NotImplementedException();
    }

    internal void Rollback()
    {
        //throw new NotImplementedException();
    }
}
public class FinancingRepositorio
{
    internal void Inserir()
    {
        //throw new NotImplementedException();
    }
}
public class AccountRepositorio
{
    internal void Inserir()
    {
        //throw new NotImplementedException();
    }
}
public class MessageBroker
{
    internal void Publish()
    {
        //throw new NotImplementedException();
    }
}


#region
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
/*
public class GlobalExceptionHandler(IHostEnvironment env, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    private const string UnhandledExceptionMsg = "An unhandled exception has occurred while executing the request.";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        //exception.AddErrorCode();
        logger.LogError(exception, exception is not null ? exception.Message : UnhandledExceptionMsg);

        ProblemDetails problemDetails = CreateProblemDetails(context, exception);
        string json = ToJson(problemDetails);

        const string contentType = "application/problem+json";
        context.Response.ContentType = contentType;
        await context.Response.WriteAsync(json, cancellationToken);

        return true;
    }

    private ProblemDetails CreateProblemDetails(in HttpContext context, in Exception exception)
    {
        int errorCode = 404;// exception.GetErrorCode();
        int statusCode = context.Response.StatusCode;
        string reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
        if (string.IsNullOrEmpty(reasonPhrase))
        {
            reasonPhrase = UnhandledExceptionMsg;
        }

        ProblemDetails problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = reasonPhrase,
            Extensions =
            {
                [nameof(errorCode)] = errorCode
            }
        };

        if (!env.IsDevelopment())
        {
            return problemDetails;
        }

        problemDetails.Detail = exception.ToString();
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        problemDetails.Extensions["data"] = exception.Data;

        return problemDetails;
    }

    private string ToJson(in ProblemDetails problemDetails)
    {
        try
        {
            return JsonSerializer.Serialize(problemDetails, SerializerOptions);
        }
        catch (Exception ex)
        {
            const string msg = "An exception has occurred while serializing error to JSON";
            logger.LogError(ex, msg);
        }

        return string.Empty;
    }
}
*/
public class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(validationException.ValidationResult, cancellationToken);

            return true;
        }

        return false;
    }
}
#endregion
using Microsoft.AspNetCore.Http.HttpResults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost(ManagementApiConfig.Endpoints.POST, PostHandler);

app.Run();

Results<Created, BadRequest> PostHandler(Financing financing)
{
    return TypedResults.Created();// "", financing.ContractNumber);
    //return TypedResults.BadRequest();
}

public partial class Program { }

public record Financing(string ContractNumber);

public static class ManagementApiConfig
{
    public static class Endpoints
    {
        public static string POST = "/weatherforecast";
    }
}

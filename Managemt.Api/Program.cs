WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    return Results.Ok();
});

app.Run();

public partial class Program { }

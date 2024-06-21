using Managemt.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServices();

WebApplication app = builder.Build();
app.AddWebApplication();
app.Run();

public partial class Program
{ }
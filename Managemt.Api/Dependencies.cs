using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using AutoMapper;

using Managemt.Api.Features.Pagamentos;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
        app.UseManagementAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
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

public static class UserAuthentication
{
    public static class Settings
    {
        public static readonly string Secret = "fedaf7d8863b48e197b9287d492b708e";
    }

    public static class TokenService
    {
        public static string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public static IServiceCollection AddUserAuthentication(this IServiceCollection services)
    {
        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        return services;
    }

    public static WebApplication UseManagementAuthorization(this WebApplication app)
    {
        app.UseAuthorization();
        return app;
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
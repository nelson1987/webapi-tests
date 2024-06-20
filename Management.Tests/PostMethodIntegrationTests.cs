using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text;
using System.Text.Json;

namespace Management.Tests;

[Trait("Category", "IntegrationTests")]
public class PostMethodIntegrationTests : IClassFixture<ApplicationTestFixture>
{
    private readonly ApplicationTestFixture _fixture;
    private readonly HttpClient _client;

    public PostMethodIntegrationTests(ApplicationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Post_Method_should_Return_Created()
    {
        //Arrange
        var contractNumber = Guid.NewGuid().ToString();
        var financing = new Financing(contractNumber);
        var serialized = JsonSerializer.Serialize(financing);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        //Act
        var result = await _client.PostAsync(ManagementApiConfig.Endpoints.POST,
            content);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_Method_Has_Servico_Without_Data_Should_Return_BadRequest()
    {
        //Arrange
        var client = _fixture.WithWebHostBuilder(x =>
                            {
                                x.ConfigureTestServices(services =>
                                {
                                    services.RemoveAll<IHandler>();
                                    services.AddScoped<IHandler, ServicoRetornandoException>();
                                });
                            })
                            .CreateClient();
        var contractNumber = Guid.NewGuid().ToString();
        var financing = new Financing(contractNumber);
        var serialized = JsonSerializer.Serialize(financing);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        //Act
        var result = await client.PostAsync(ManagementApiConfig.Endpoints.POST,
            content);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
}
public class ApplicationTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public Task InitializeAsync()
    {
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }
}
public class ServicoRetornandoException : IHandler
{
    public void Inserir(Financing financing)
    {
        throw new Exception();
    }
}
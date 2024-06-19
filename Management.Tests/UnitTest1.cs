using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;

namespace Management.Tests;
public class UnitTest1 : IClassFixture<ApplicationTestFixture>
{
    private readonly ApplicationTestFixture _fixture;
    private readonly HttpClient _client;

    public UnitTest1(ApplicationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Post_Method_Return_Created()
    {
        //Arrange
        string contractNumber = Guid.NewGuid().ToString();
        Financing financing = new Financing(contractNumber);
        string serialized = JsonSerializer.Serialize(financing);
        StringContent content = new StringContent(serialized, Encoding.UTF8, "application/json");
        //Act
        HttpResponseMessage result = await _client.PostAsync(ManagementApiConfig.Endpoints.POST,
            content);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
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
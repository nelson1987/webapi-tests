using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

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
    public async Task Get_Method_Return_Ok()
    {
        HttpResponseMessage result = await _client.GetAsync("/weatherforecast");
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
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
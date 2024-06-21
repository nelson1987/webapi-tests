using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Managemt.Api.Core.Domain.Entities;
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
    private readonly IFixture _moqFixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly ApplicationTestFixture _fixture;
    private readonly HttpClient _client;

    public PostMethodIntegrationTests(ApplicationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Get_Method_should_Return_Created()
    {
        //Arrange
        var contractNumber = Guid.NewGuid().ToString();
        //Act
        var result = await _client.GetAsync(string.Format(ManagementApiConfig.Endpoints.GET, contractNumber));
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_Method_should_Return_Created()
    {
        //Arrange
        var contractNumber = Guid.NewGuid().ToString();
        var financing = _moqFixture.Build<FinanciamentoCommand>()
            .With(x => x.NumeroContrato, contractNumber)
            .Create();
        var serialized = JsonSerializer.Serialize(financing);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        //Act
        var result = await _client.PostAsync(ManagementApiConfig.Endpoints.POST,
            content);
        //Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Post_Method_Exists_Financing_Contract_should_Return_Created()
    {
        //Arrange
        var client = _fixture.WithWebHostBuilder(x =>
                            {
                                x.ConfigureTestServices(services =>
                                {
                                    services.RemoveAll<IProdutoRepository>();
                                    services.AddScoped<IProdutoRepository, RepositoryComNumeroContratoExistente>();
                                });
                            })
                            .CreateClient();
        var contractNumber = Guid.NewGuid().ToString();
        var financing = _moqFixture.Build<FinanciamentoCommand>()
            .With(x => x.NumeroContrato, contractNumber)
            .Create();
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
                                    services.RemoveAll<IProdutoRepository>();
                                    services.AddScoped<IProdutoRepository, RepositoryComException>();
                                });
                            })
                            .CreateClient();
        var contractNumber = Guid.NewGuid().ToString();
        var financing = _moqFixture.Build<FinanciamentoCommand>()
            .With(x => x.NumeroContrato, contractNumber)
            .Create();
        var serialized = JsonSerializer.Serialize(financing);
        var content = new StringContent(serialized, Encoding.UTF8, "application/json");
        //Act
        var result = await client.PostAsync(ManagementApiConfig.Endpoints.POST, content);
        //Assert
        //result.Should().. <NotImplementedException>();
        //result.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
}

public class ApplicationTestFixture : WebApplicationFactory<Program>
{
}

public class RepositoryComNumeroContratoExistente : IProdutoRepository
{
    public Task<int> Count(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> Count(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Financiamento entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento?> Find(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Financiamento?>> FindAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<Financiamento?>> FindAll(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento?> FindById(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Financiamento?> GetByContractNumber(string contractNumber, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(new Financiamento() { Id = 1, ContractNumber = contractNumber });
    }

    public Task<Financiamento> Insert(Financiamento entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity);
    }

    public Task Update(Financiamento entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public class RepositoryComException : IProdutoRepository
{
    public Task<int> Count(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> Count(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Financiamento entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento?> Find(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Financiamento?>> FindAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<Financiamento?>> FindAll(Func<Financiamento, bool> expression, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento?> FindById(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Financiamento?> GetByContractNumber(string contractNumber, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Financiamento> Insert(Financiamento entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity);
    }

    public Task Update(Financiamento entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
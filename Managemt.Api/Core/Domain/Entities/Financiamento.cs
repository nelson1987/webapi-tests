namespace Managemt.Api.Core.Domain.Entities;

public record Financiamento
{
    public int Id { get; set; }
    public string? ContractNumber { get; set; }
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
    public int IdEstoque { get; set; }
    public Estoque? Estoque { get; set; }
}
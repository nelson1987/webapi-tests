namespace Managemt.Api.Core.Domain.Entities;

public class Estoque
{
    public int IdEstoque { get; set; }
    public string? Nome { get; set; }
    public List<Financiamento>? Produtos { get; set; }
}

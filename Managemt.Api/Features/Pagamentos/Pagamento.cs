namespace Managemt.Api.Features.Pagamentos;

public class Pagamento
{
    public int Id { get; set; }
    public string? NumeroContrato { get; set; }
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
    public int IdEstoque { get; set; }

    public Pagamento(string numeroContrato, decimal preco, int quantidade)
    {
        NumeroContrato = numeroContrato;
        Preco = preco;
        Quantidade = quantidade;
    }
}
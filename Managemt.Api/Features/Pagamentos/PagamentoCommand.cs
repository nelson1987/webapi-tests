namespace Managemt.Api.Features.Pagamentos;

public record PagamentoCommand(string NumeroContrato, decimal Preco, int Quantidade);

//using Microsoft.AspNetCore.Mvc;

//namespace Managemt.Api.Features.Financings;

//[ApiController]
//[Consumes("application/json")]
//[Route("[Controller]")]
//public class FinanciamentosController : ControllerBase
//{
//    [HttpGet("/{contractNumber}")]
//    public async Task<IActionResult> Get([FromServices] IProdutoRepository repository, string contractNumber, CancellationToken cancellationToken = default)
//    {
//        var entities = await repository.Find(x => x.ContractNumber == contractNumber, cancellationToken);
//        return Ok(entities);
//    }

//    [HttpPost]
//    public async Task<IActionResult> Post([FromServices] IFinancingCommandHandler handler, [FromBody] FinanciamentoCommand financing, CancellationToken cancellationToken = default)
//    {
//        var response = await handler.Handle(financing, cancellationToken);
//        return StatusCode((int)response.StatusCode, response.Response);
//    }
//}
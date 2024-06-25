using System.Net;

using FluentValidation;

using Managemt.Api.Extensions;
using Managemt.Api.Features.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Managemt.Api.Features.Pagamentos;

public class Usuario
{
    public string Nome { get; set; }
}

public class Endereco
{
    public string Logradouro { get; set; }
}

[ApiController]
[Consumes("application/json")]
[Route("/pagamentos")]
public class PagamentoController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IPagamentoHandler _pagamentoHandler;
    private readonly IValidator<PagamentoCommand> _validator;

    public PagamentoController(IPagamentoHandler pagamentoHandler, IValidator<PagamentoCommand> validator, ITokenService tokenService)
    {
        _pagamentoHandler = pagamentoHandler;
        _validator = validator;
        _tokenService = tokenService;
    }

    public record LoginAccountCommand(string Username, string Password);

    [AllowAnonymous]
    [HttpGet("/token")]
    public async Task<IActionResult> GetToken(CancellationToken cancellationToken = default)
    {
        var user = new User { Id = 1, Username = "Batman", Password = "Batman", Role = Settings.AuthorizationTypes.Employee };
        var token = _tokenService.GenerateToken(user);
        return Ok(token);
    }

    [Authorize(Roles = Settings.AuthorizationTypes.Employee)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PagamentoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            //var identity = (ClaimsIdentity?)User.Identity!;
            var validation = await _validator.ValidateAsync(command, cancellationToken);
            if (validation.IsInvalid())
                return ValidationProblem(modelStateDictionary: validation.ToModelState(), statusCode: (int)HttpStatusCode.BadRequest);

            Pagamento pagamento = await _pagamentoHandler.HandleAsync(command, cancellationToken);
            return Created($"/{pagamento.Id}", null);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}
namespace Managemt.Api.Features.Users;

public interface ITokenService
{
    string GenerateToken(User user);
}
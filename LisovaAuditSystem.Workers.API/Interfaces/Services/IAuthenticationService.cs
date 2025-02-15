using LisovaAuditSystem.Workers.API.Dtos;

namespace LisovaAuditSystem.Workers.API.Interfaces.Services;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(UserRegisterCredentialsDto userRegisterCredentials);

    Task<string> LoginAsync(UserLoginCredentialsDto userLoginCredentials);
}

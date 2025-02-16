using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;
using LisovaAuditSystem.Workers.API.Interfaces.Services;

namespace LisovaAuditSystem.Workers.API.Services;

public class AuthenticationService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerationService tokenGenerationService)
    : IAuthenticationService
{
    public async Task<string> RegisterAsync(UserRegisterCredentialsDto userRegisterCredentials)
    {
        string passwordHash = passwordHasher.Hash(userRegisterCredentials.Password);

        UserDto user =
            new(
                id: Guid.NewGuid(),
                userName: userRegisterCredentials.UserName,
                email: userRegisterCredentials.Email,
                passwordHash: passwordHash);
        await userRepository.AddAsync(user);

        return tokenGenerationService.Generate(
            userId: user.Id,
            userName: user.UserName,
            userEmail: user.Email);
    }

    public async Task<string> LoginAsync(UserLoginCredentialsDto userLoginCredentials)
    {
        UserDto? user = await userRepository.GetByEmailAsync(userLoginCredentials.Email);

        if (user is null)
        {
            throw new InvalidOperationException($"User with email: {userLoginCredentials.Email} not found.");
        }

        bool isValidPassword = passwordHasher.Verify(userLoginCredentials.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            throw new InvalidOperationException("User password not valid!");
        }

        return tokenGenerationService.Generate(
            userId: user.Id,
            userName: user.UserName,
            userEmail: user.Email);
    }
}

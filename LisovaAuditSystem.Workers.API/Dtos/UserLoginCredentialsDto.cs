namespace LisovaAuditSystem.Workers.API.Dtos;

public class UserLoginCredentialsDto(string email, string password)
{
    public string Email { get; set; } = email;

    public string Password { get; set; } = password;
}

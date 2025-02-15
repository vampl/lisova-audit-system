namespace LisovaAuditSystem.Workers.API.Dtos;

public class UserRegisterCredentialsDto(string email, string username, string password)
{
    public string UserName { get; set; } = username;

    public string Email { get; set; } = email;

    public string Password { get; set; } = password;
}

namespace LisovaAuditSystem.Workers.API.Dtos;

public class UserDto(Guid id, string userName, string email, string passwordHash)
{
    public Guid Id { get; set; } = id;

    public string UserName { get; set; } = userName;

    public string Email { get; set; } = email;

    public string PasswordHash { get; set; } = passwordHash;
}

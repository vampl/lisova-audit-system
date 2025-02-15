namespace LisovaAuditSystem.Workers.API.Entities;

public class User(Guid id, string userName, string email, string passwordHash)
{
    public Guid Id { get; private set; } = id;

    public string UserName { get; private set; } = userName;

    public string Email { get; private set; } = email;

    public string PasswordHash { get; private set; } = passwordHash;
}

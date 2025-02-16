namespace LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;

public interface IJwtTokenGenerationService
{
    string Generate(Guid userId, string userName, string userEmail);
}

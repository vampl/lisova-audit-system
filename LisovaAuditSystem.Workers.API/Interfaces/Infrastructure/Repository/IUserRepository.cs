using LisovaAuditSystem.Workers.API.Dtos;

namespace LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;

public interface IUserRepository : IRepository<UserDto>
{
    Task<UserDto?> GetByEmailAsync(string email, bool asNoTracking = true);
}

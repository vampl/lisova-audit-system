namespace LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;

public interface IRepository<TDto>
{
    Task<IList<TDto>> GetAllAsync(bool asNoTracking = true);

    Task<TDto?> GetByIdAsync(Guid id, bool asNoTracking = true);

    Task AddAsync(TDto addDto);

    Task UpdateAsync(TDto updateDto);

    Task DeleteAsync(Guid id);
}

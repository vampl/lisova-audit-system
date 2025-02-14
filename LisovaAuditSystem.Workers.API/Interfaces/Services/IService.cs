namespace LisovaAuditSystem.Workers.API.Interfaces.Services;

public interface IService<TDto>
{
    Task<IList<TDto>> ReadAllAsync();

    Task<TDto> ReadByIdAsync(Guid workerId);

    Task<Guid> CreateAsync(TDto addWorker);

    Task EditAsync(TDto updateWorker);

    Task RemoveAsync(Guid workerId);
}

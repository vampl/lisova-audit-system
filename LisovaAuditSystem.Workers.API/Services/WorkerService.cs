using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;
using LisovaAuditSystem.Workers.API.Interfaces.Services;

namespace LisovaAuditSystem.Workers.API.Services;

public class WorkerService(IWorkerRepository repository) : IWorkerService
{
    public async Task<IList<WorkerDto>> ReadAllAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<WorkerDto> ReadByIdAsync(Guid workerId)
    {
        return await repository.GetByIdAsync(workerId) ??
               throw new InvalidOperationException($"Worker with Id: {workerId} not found.");
    }

    public async Task<Guid> CreateAsync(WorkerDto addWorker)
    {
        addWorker.Id = Guid.NewGuid();

        addWorker.ValidateAndThrow();

        await repository.AddAsync(addWorker);

        return addWorker.Id;
    }

    public async Task EditAsync(WorkerDto updateWorker)
    {
        updateWorker.ValidateAndThrow();

        await repository.UpdateAsync(updateWorker);
    }

    public async Task RemoveAsync(Guid workerId)
    {
        await repository.DeleteAsync(workerId);
    }
}

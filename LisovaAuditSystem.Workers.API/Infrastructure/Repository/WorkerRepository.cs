using LisovaAuditSystem.Workers.API.Common.Extensions.Mappings;
using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LisovaAuditSystem.Workers.API.Infrastructure.Repository;

public class WorkerRepository(WorkersContext context) : IWorkerRepository
{
    private readonly DbSet<Worker> _set = context.Set<Worker>();

    public async Task<IList<WorkerDto>> GetAllAsync(bool asNoTracking = true)
    {
        IQueryable<Worker> query = _set;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.Select(worker => worker.ToDto()).ToListAsync();
    }

    public async Task<WorkerDto?> GetByIdAsync(Guid id, bool asNoTracking = true)
    {
        IQueryable<Worker> query = _set;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return (await query.FirstOrDefaultAsync(worker => worker.Id == id))?.ToDto();
    }

    public async Task AddAsync(WorkerDto addDto)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        if (await _set.AnyAsync(worker => worker.Id == addDto.Id))
        {
            throw new InvalidOperationException($"Worker with id: {addDto.Id} already exists.");
        }

        try
        {
            await _set.AddAsync(addDto.ToEntity());

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                $"Unable to add worker with id: {addDto.Id}.",
                innerException: exception);
        }
    }

    public async Task UpdateAsync(WorkerDto updateDto)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        Worker workerToUpdate =
            await _set.FirstOrDefaultAsync(worker => worker.Id == updateDto.Id) ??
            throw new InvalidOperationException($"Worker with id: {updateDto.Id} not found.");

        try
        {
            _set.Entry(workerToUpdate).CurrentValues.SetValues(updateDto);
            _set.Entry(workerToUpdate).State = EntityState.Modified;

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                $"Unable to update worker with id: {updateDto.Id}.",
                innerException: exception);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        Worker workerToDelete =
            await _set.FirstOrDefaultAsync(worker => worker.Id == id) ??
            throw new InvalidOperationException($"Worker with id: {id} already exists.");

        try
        {
            _set.Remove(workerToDelete);

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                $"Unable to delete worker with id: {id}.",
                innerException: exception);
        }
    }
}

using LisovaAuditSystem.Workers.API.Common.Extensions.Mappings;
using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LisovaAuditSystem.Workers.API.Infrastructure.Repository;

public class UserRepository(WorkersContext context) : IUserRepository
{
    private readonly DbSet<User> _set = context.Set<User>();

    public async Task<IList<UserDto>> GetAllAsync(bool asNoTracking = true)
    {
        IQueryable<User> query = _set;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.Select(user => user.ToDto()).ToListAsync();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, bool asNoTracking = true)
    {
        IQueryable<User> query = _set;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return (await query.FirstOrDefaultAsync(user => user.Id == id))?.ToDto();
    }

    public async Task AddAsync(UserDto addDto)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        if (await _set.AnyAsync(user => user.Id == addDto.Id))
        {
            throw new InvalidOperationException($"User with id: {addDto.Id} already exists.");
        }

        if (await _set.AnyAsync(user => user.UserName == addDto.UserName))
        {
            throw new InvalidOperationException($"User with username: {addDto.UserName} already exists.");
        }

        if (await _set.AnyAsync(user => user.Email == addDto.Email))
        {
            throw new InvalidOperationException($"User with email: {addDto.Email} already exists.");
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
                $"Unable to add user with id: {addDto.Id}.",
                innerException: exception);
        }
    }

    public async Task UpdateAsync(UserDto updateDto)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        User userToUpdate =
            await _set.FirstOrDefaultAsync(user => user.Id == updateDto.Id) ??
            throw new InvalidOperationException($"User with id: {updateDto.Id} not found.");

        try
        {
            _set.Entry(userToUpdate).CurrentValues.SetValues(updateDto);
            _set.Entry(userToUpdate).State = EntityState.Modified;

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                $"Unable to update user with id: {updateDto.Id}.",
                innerException: exception);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        User userToDelete =
            await _set.FirstOrDefaultAsync(user => user.Id == id) ??
            throw new InvalidOperationException($"User with id: {id} already exists.");

        try
        {
            _set.Remove(userToDelete);

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                $"Unable to delete user with id: {id}.",
                innerException: exception);
        }
    }
    public async Task<UserDto?> GetByEmailAsync(string email, bool asNoTracking = true)
    {
        IQueryable<User> query = _set;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return (await query.SingleOrDefaultAsync(user => user.Email == email))?.ToDto();
    }
}

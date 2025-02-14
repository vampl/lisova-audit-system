using FluentAssertions;

using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;
using LisovaAuditSystem.Workers.API.Infrastructure;
using LisovaAuditSystem.Workers.API.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LisovaAuditSystem.Workers.Data.Tests.Infrastructure.Repository;

public class WorkerRepositoryTests
{
    private readonly WorkersContext _context;
    private readonly WorkerRepository _repository;

    public WorkerRepositoryTests()
    {
        DbContextOptions<WorkersContext> options =
            new DbContextOptionsBuilder<WorkersContext>()
                .UseInMemoryDatabase(databaseName: $"Database-{Guid.NewGuid()}")
                .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        _context = new WorkersContext(options);
        _repository = new WorkerRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsAllWorkers()
    {
        // Arrange
        IList<Worker> workers =
        [
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null),
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Jean",
                birthDate: DateTimeOffset.Now.AddYears(-30),
                phone: "380932103079",
                email: "jean.doe@gmail.com")
        ];

        _context.Workers.AddRange(workers);
        await _context.SaveChangesAsync();

        IList<WorkerDto> expected =
            workers.Select(
                    worker =>
                        new WorkerDto(
                            id: worker.Id,
                            lastName: worker.LastName,
                            name: worker.Name,
                            birthDate: worker.BirthDate,
                            phone: worker.Phone,
                            email: worker.Email))
                .ToList();

        // Act
        IList<WorkerDto> actual = await _repository.GetAllAsync();

        // Assert
        actual.Should().NotBeEmpty().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsEmptyCollection()
    {
        // Arrange & Act
        IList<WorkerDto> actual = await _repository.GetAllAsync();

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalledWithValidId_ReturnsWorker()
    {
        // Arrange
        var id = Guid.NewGuid();

        Worker worker =
            new(
                id: id,
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        await _context.Workers.AddAsync(worker);
        await _context.SaveChangesAsync();

        WorkerDto expected =
            new(
                id: worker.Id,
                lastName: worker.LastName,
                name: worker.Name,
                birthDate: worker.BirthDate,
                phone: worker.Phone,
                email: worker.Email);

        // Act
        WorkerDto? actual = await _repository.GetByIdAsync(id);

        // Assert
        actual.Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalledWithInvalidId_ReturnsNull()
    {
        // Arrange & Act
        WorkerDto? actual = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCalledWithValidData_AddsWorker()
    {
        // Arrange
        WorkerDto addWorker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        Worker expected =
            new(
                id: addWorker.Id,
                lastName: addWorker.LastName,
                name: addWorker.Name,
                birthDate: addWorker.BirthDate,
                phone: addWorker.Phone,
                email: addWorker.Email);

        // Act
        await _repository.AddAsync(addWorker);

        // Assert
        Worker? actual = await _context.Workers.FirstOrDefaultAsync(worker => worker.Id == addWorker.Id);

        actual.Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AddAsync_WhenCalledWithInvalidData_ThrowsInvalidOperationException()
    {
        // Arrange
        Worker originalWorker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        await _context.Workers.AddAsync(originalWorker);
        await _context.SaveChangesAsync();

        WorkerDto existingWorker =
            new(
                id: originalWorker.Id,
                lastName: originalWorker.LastName,
                name: originalWorker.Name,
                birthDate: originalWorker.BirthDate,
                phone: originalWorker.Phone,
                email: originalWorker.Email);

        // Act & Assert
        await FluentActions.Invoking(() => _repository.AddAsync(existingWorker))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenCalledWithValidData_UpdatesWorker()
    {
        // Arrange
        Worker originalWorker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        await _context.Workers.AddAsync(originalWorker);
        await _context.SaveChangesAsync();

        WorkerDto updateWorker =
            new(
                id: originalWorker.Id,
                lastName: "John",
                name: originalWorker.Name,
                birthDate: originalWorker.BirthDate,
                phone: originalWorker.Phone,
                email: originalWorker.Email);

        Worker expected =
            new(
                id: updateWorker.Id,
                lastName: updateWorker.LastName,
                name: updateWorker.Name,
                birthDate: updateWorker.BirthDate,
                phone: updateWorker.Phone,
                email: updateWorker.Email);

        // Act
        await _repository.UpdateAsync(updateWorker);

        // Assert
        Worker? actual = await _context.Workers.FirstOrDefaultAsync(worker => worker.Id == updateWorker.Id);

        actual.Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateAsync_WhenCalledWithInvalidData_ThrowsInvalidOperationException()
    {
        // Arrange
        Worker originalWorker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        await _context.Workers.AddAsync(originalWorker);
        await _context.SaveChangesAsync();

        WorkerDto nonExistingWorker =
            new(
                id: Guid.NewGuid(),
                lastName: originalWorker.LastName,
                name: originalWorker.Name,
                birthDate: originalWorker.BirthDate,
                phone: originalWorker.Phone,
                email: originalWorker.Email);

        // Act & Assert
        await FluentActions.Invoking(() => _repository.UpdateAsync(nonExistingWorker))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalledWithValidId_DeletesWorker()
    {
        // Arrange
        Worker originalWorker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        await _context.Workers.AddAsync(originalWorker);
        await _context.SaveChangesAsync();

        Guid deleteId = originalWorker.Id;

        // Act
        await _repository.DeleteAsync(deleteId);

        // Assert
        Worker? actual = await _context.Workers.FirstOrDefaultAsync(worker => worker.Id == deleteId);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalledWithInvalidId_ThrowsException()
    {
        // Arrange
        var nonExistingWorkerId = Guid.NewGuid();

        // Act & Assert
        await FluentActions.Invoking(() => _repository.DeleteAsync(nonExistingWorkerId))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }
}

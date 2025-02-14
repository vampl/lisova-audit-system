using FluentAssertions;

using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;
using LisovaAuditSystem.Workers.API.Services;

using Moq;

namespace LisovaAuditSystem.Workers.Business.Tests.Services;

public class WorkerServiceTests
{
    private readonly Mock<IWorkerRepository> _workerRepositoryMock;
    private readonly WorkerService _service;

    public WorkerServiceTests()
    {
        _workerRepositoryMock = new Mock<IWorkerRepository>();
        _service = new WorkerService(_workerRepositoryMock.Object);
    }

    [Fact]
    public async Task ReadAllAsync_WhenCalled_CallsRepositoryGetAllAsyncAndReturnsAllWorkers()
    {
        // Arrange
        IList<WorkerDto> workers =
        [
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-45),
                phone: "380931792100",
                email: "joe@doe.com")
        ];

        _workerRepositoryMock.Setup(repository => repository.GetAllAsync(It.IsAny<bool>()))
            .ReturnsAsync(workers);

        // Act
        IList<WorkerDto> actual = await _service.ReadAllAsync();

        // Assert
        _workerRepositoryMock.Verify(
            repository => repository.GetAllAsync(It.Is<bool>(argument => argument.Equals(true))),
            Times.Once);

        actual.Should().BeEquivalentTo(workers);
    }

    [Fact]
    public async Task ReadByIdAsync_WhenCalled_CallsRepositoryGetByIdAsyncAndReturnsWorker()
    {
        // Arrange
        var workerId = Guid.NewGuid();

        WorkerDto expected =
            new(
                id: workerId,
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-45),
                phone: "380931792100",
                email: "joe@doe.com");

        _workerRepositoryMock.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(expected);

        // Act
        WorkerDto actual = await _service.ReadByIdAsync(workerId);

        // Assert
        _workerRepositoryMock.Verify(
            repository =>
                repository.GetByIdAsync(
                    It.Is<Guid>(argument => argument.Equals(workerId)),
                    It.Is<bool>(argument => argument.Equals(true))),
            Times.Once);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_CallsValidationAndRepositoryAddAsyncAndSaveChangesAsyncReturnsNewWorkerId()
    {
        // Arrange
        WorkerDto addWorker =
            new(
                id: Guid.Empty,
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-45),
                phone: "380931792100",
                email: "joe@doe.com");

        _workerRepositoryMock.Setup(repository => repository.AddAsync(It.IsAny<WorkerDto>()))
            .Returns(Task.CompletedTask);

        // Act
        Guid actual = await _service.CreateAsync(addWorker);

        // Assert
        _workerRepositoryMock.Verify(
            repository => repository.AddAsync(It.Is<WorkerDto>(argument => argument.Equals(addWorker))),
            Times.Once);

        actual.Should().NotBeEmpty();
    }

    [Fact]
    public async Task EditAsync_WhenCalled_CallsValidationAndRepositoryUpdateAsyncAndSaveChangesAsync()
    {
        // Arrange
        WorkerDto updateWorker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-45),
                phone: "380931792100",
                email: "joe@doe.com");

        _workerRepositoryMock.Setup(repository => repository.UpdateAsync(It.IsAny<WorkerDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.EditAsync(updateWorker);

        // Assert
        _workerRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.Is<WorkerDto>(argument => argument.Equals(updateWorker))),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenCalled_CallsRepositoryDeleteAsyncAndSaveChangesAsync()
    {
        // Arrange
        var workerId = Guid.NewGuid();

        _workerRepositoryMock.Setup(repository => repository.DeleteAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.RemoveAsync(workerId);

        // Assert
        _workerRepositoryMock.Verify(
            repository => repository.DeleteAsync(It.Is<Guid>(argument => argument.Equals(workerId))),
            Times.Once);
    }
}

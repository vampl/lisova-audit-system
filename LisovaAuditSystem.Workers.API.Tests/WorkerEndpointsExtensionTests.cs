using FluentAssertions;

using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Endpoints;
using LisovaAuditSystem.Workers.API.Models.Payloads;
using LisovaAuditSystem.Workers.API.Models.Requests;
using LisovaAuditSystem.Workers.API.Models.Responses;
using LisovaAuditSystem.Workers.API.Services;

using Microsoft.AspNetCore.Http.HttpResults;

using Moq;

namespace LisovaAuditSystem.Workers.API.Tests;

public class WorkerEndpointsExtensionTests
{
    private readonly Mock<IWorkerService> _workerServiceMock;

    public WorkerEndpointsExtensionTests()
    {
        _workerServiceMock = new Mock<IWorkerService>();
    }

    [Fact]
    public async Task GetParameterless_WhenCalled_Returns200OkWithWorkers()
    {
        // Arrange
        IList<WorkerDto> workers =
        [
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380935020369",
                email: null)
        ];

        var expected =
            new GetWorkersResponse(
                workers.Select(
                        worker =>
                            new GetWorkerPayload(
                                id: worker.Id,
                                lastName: worker.LastName,
                                name: worker.Name,
                                birthDate: worker.BirthDate,
                                phone: worker.Phone,
                                email: worker.Email))
                    .ToList());

        _workerServiceMock.Setup(service => service.ReadAllAsync())
            .ReturnsAsync(workers);

        // Act
        Ok<GetWorkersResponse> actual = await WorkerEndpointsExtension.GetWorkersAsync(_workerServiceMock.Object);

        // Assert
        actual.Should().BeOfType<Ok<GetWorkersResponse>>();
        actual.Value.Should().BeEquivalentTo(expected);

        _workerServiceMock.Verify(
            service => service.ReadAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetWithIdParameter_WhenCalled_Returns200OkWithWorker()
    {
        // Arrange
        var workerId = Guid.NewGuid();

        WorkerDto worker =
            new(
                id: workerId,
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380935020369",
                email: null);

        var expectedResponse =
            new GetWorkerResponse(
                new GetWorkerPayload(
                    id: worker.Id,
                    lastName: worker.LastName,
                    name: worker.Name,
                    birthDate: worker.BirthDate,
                    phone: worker.Phone,
                    email: worker.Email));

        _workerServiceMock.Setup(service => service.ReadByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(worker);

        // Act
        Results<Ok<GetWorkerResponse>, NotFound<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.GetWorkerAsync(_workerServiceMock.Object, workerId);

        // Assert
        actual.Result.Should().BeOfType<Ok<GetWorkerResponse>>();
        actual.Result.As<Ok<GetWorkerResponse>>().Value.Should().BeEquivalentTo(expectedResponse);

        _workerServiceMock.Verify(
            service => service.ReadByIdAsync(It.Is<Guid>(argument => argument == workerId)),
            Times.Once);
    }

    [Fact]
    public async Task GetWithIdParameter_WhenCalled_Returns404NoFoundWithMessage()
    {
        // Arrange
        var workerId = Guid.NewGuid();

        string expectedMessage = $"Record with Id: {workerId} not found";

        _workerServiceMock.Setup(service => service.ReadByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<Ok<GetWorkerResponse>, NotFound<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.GetWorkerAsync(_workerServiceMock.Object, workerId);

        // Assert
        actual.Result.Should().BeOfType<NotFound<string>>();
        actual.Result.As<NotFound<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _workerServiceMock.Verify(
            service => service.ReadByIdAsync(It.Is<Guid>(argument => argument == workerId)),
            Times.Once);
    }

    [Fact]
    public async Task PostWithValidRequest_WhenCalled_ReturnsCreatedWithWorkerId()
    {
        // Arrange
        PostWorkerRequest request =
            new(
                new PostWorkerPayload(
                    lastName: "Doe",
                    name: "Joe",
                    birthDate: DateTimeOffset.Now.AddYears(-20),
                    phone: "380935020369",
                    email: null));

        var workerId = Guid.NewGuid();

        PostWorkerResponse expected = new(workerId);

        _workerServiceMock.Setup(service => service.CreateAsync(It.IsAny<WorkerDto>()))
            .ReturnsAsync(workerId);

        // Act
        Results<Created<PostWorkerResponse>, BadRequest<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.PostWorkerAsync(_workerServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<Created<PostWorkerResponse>>();
        actual.Result.As<Created<PostWorkerResponse>>().Value.Should().BeEquivalentTo(expected);

        _workerServiceMock.Verify(
            service =>
                service.CreateAsync(
                    It.Is<WorkerDto>(
                        argument =>
                            argument.LastName.Equals(request.PostWorkerPayload.LastName) &&
                            argument.Name.Equals(request.PostWorkerPayload.Name) &&
                            argument.BirthDate.Equals(request.PostWorkerPayload.BirthDate) &&
                            argument.Phone.Equals(request.PostWorkerPayload.Phone) &&
                            argument.Email == request.PostWorkerPayload.Email)),
            Times.Once());
    }

    [Fact]
    public async Task PostWithInvalidRequest_WhenCalled_Returns400BadRequestWithMessage()
    {
        // Arrange
        PostWorkerRequest request =
            new(
                new PostWorkerPayload(
                    lastName: "Doe",
                    name: "Joe",
                    birthDate: DateTimeOffset.Now.AddYears(-20),
                    phone: "",
                    email: null));

        string expectedMessage = $"Validation failed:\n{nameof(request.PostWorkerPayload.Phone)}";

        _workerServiceMock.Setup(service => service.CreateAsync(It.IsAny<WorkerDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<Created<PostWorkerResponse>, BadRequest<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.PostWorkerAsync(_workerServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<BadRequest<string>>();
        actual.Result.As<BadRequest<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _workerServiceMock.Verify(
            service =>
                service.CreateAsync(
                    It.Is<WorkerDto>(
                        argument =>
                            argument.LastName.Equals(request.PostWorkerPayload.LastName) &&
                            argument.Name.Equals(request.PostWorkerPayload.Name) &&
                            argument.BirthDate.Equals(request.PostWorkerPayload.BirthDate) &&
                            argument.Phone.Equals(request.PostWorkerPayload.Phone) &&
                            argument.Email == request.PostWorkerPayload.Email)),
            Times.Once());
    }

    [Fact]
    public async Task PostWithExistingModel_WhenCalled_Returns400BadRequestWithMessage()
    {
        // Arrange
        PostWorkerRequest request =
            new(
                new PostWorkerPayload(
                    lastName: "Doe",
                    name: "Joe",
                    birthDate: DateTimeOffset.Now.AddYears(-20),
                    phone: "380935020369",
                    email: null));

        var workerId = Guid.NewGuid();

        string expectedMessage = $"Record with Id: {workerId} already exists";

        _workerServiceMock.Setup(service => service.CreateAsync(It.IsAny<WorkerDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<Created<PostWorkerResponse>, BadRequest<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.PostWorkerAsync(_workerServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<BadRequest<string>>();
        actual.Result.As<BadRequest<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _workerServiceMock.Verify(
            service =>
                service.CreateAsync(
                    It.Is<WorkerDto>(
                        argument =>
                            argument.LastName.Equals(request.PostWorkerPayload.LastName) &&
                            argument.Name.Equals(request.PostWorkerPayload.Name) &&
                            argument.BirthDate.Equals(request.PostWorkerPayload.BirthDate) &&
                            argument.Phone.Equals(request.PostWorkerPayload.Phone) &&
                            argument.Email == request.PostWorkerPayload.Email)),
            Times.Once());
    }

    [Fact]
    public async Task PutWithValidRequest_WhenCalled_Returns204NoContent()
    {
        // Arrange
        PutWorkerRequest request =
            new(
                new PutWorkerPayload(
                    id: Guid.NewGuid(),
                    lastName: "Doe",
                    name: "Joe",
                    birthDate: DateTimeOffset.Now.AddYears(-20),
                    phone: "380935020369",
                    email: null));

        _workerServiceMock.Setup(service => service.EditAsync(It.IsAny<WorkerDto>()))
            .Returns(Task.CompletedTask);

        // Act
        Results<NoContent, BadRequest<string>, NotFound<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.PutWorkerAsync(_workerServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<NoContent>();

        _workerServiceMock.Verify(
            service =>
                service.EditAsync(
                    It.Is<WorkerDto>(
                        argument =>
                            argument.Id.Equals(request.PutWorkerPayload.Id) &&
                            argument.LastName.Equals(request.PutWorkerPayload.LastName) &&
                            argument.Name.Equals(request.PutWorkerPayload.Name) &&
                            argument.BirthDate.Equals(request.PutWorkerPayload.BirthDate) &&
                            argument.Phone.Equals(request.PutWorkerPayload.Phone) &&
                            argument.Email == request.PutWorkerPayload.Email)),
            Times.Once());
    }

    [Fact]
    public async Task PutWithInvalidRequest_WhenCalled_Returns400BadRequestWithMessage()
    {
        // Arrange
        PutWorkerRequest request =
            new(
                new PutWorkerPayload(
                    id: Guid.NewGuid(),
                    lastName: "Doe",
                    name: "Joe",
                    birthDate: DateTimeOffset.Now.AddYears(-20),
                    phone: "",
                    email: null));

        string expectedMessage = $"Validation failed:\n{nameof(request.PutWorkerPayload.Phone)}";

        _workerServiceMock.Setup(service => service.EditAsync(It.IsAny<WorkerDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<NoContent, BadRequest<string>, NotFound<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.PutWorkerAsync(_workerServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<BadRequest<string>>();
        actual.Result.As<BadRequest<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _workerServiceMock.Verify(
            service =>
                service.EditAsync(
                    It.Is<WorkerDto>(
                        argument =>
                            argument.Id.Equals(request.PutWorkerPayload.Id) &&
                            argument.LastName.Equals(request.PutWorkerPayload.LastName) &&
                            argument.Name.Equals(request.PutWorkerPayload.Name) &&
                            argument.BirthDate.Equals(request.PutWorkerPayload.BirthDate) &&
                            argument.Phone.Equals(request.PutWorkerPayload.Phone) &&
                            argument.Email == request.PutWorkerPayload.Email)),
            Times.Once());
    }

    [Fact]
    public async Task PutWithNonExistingModel_WhenCalled_Returns404NotFoundWithMessage()
    {
        // Arrange
        PutWorkerRequest request =
            new(
                new PutWorkerPayload(
                    id: Guid.NewGuid(),
                    lastName: "Doe",
                    name: "Joe",
                    birthDate: DateTimeOffset.Now.AddYears(-20),
                    phone: "",
                    email: null));

        string expectedMessage = $"Record with Id: {request.PutWorkerPayload.Id} not found";

        _workerServiceMock.Setup(service => service.EditAsync(It.IsAny<WorkerDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<NoContent, BadRequest<string>, NotFound<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.PutWorkerAsync(_workerServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<NotFound<string>>();
        actual.Result.As<NotFound<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _workerServiceMock.Verify(
            service =>
                service.EditAsync(
                    It.Is<WorkerDto>(
                        argument =>
                            argument.Id.Equals(request.PutWorkerPayload.Id) &&
                            argument.LastName.Equals(request.PutWorkerPayload.LastName) &&
                            argument.Name.Equals(request.PutWorkerPayload.Name) &&
                            argument.BirthDate.Equals(request.PutWorkerPayload.BirthDate) &&
                            argument.Phone.Equals(request.PutWorkerPayload.Phone) &&
                            argument.Email == request.PutWorkerPayload.Email)),
            Times.Once());
    }

    [Fact]
    public async Task DeleteWithIdParameter_WhenCalled_Returns204NoContent()
    {
        // Arrange
        var workerId = Guid.NewGuid();

        _workerServiceMock.Setup(service => service.RemoveAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        Results<NoContent, BadRequest<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.DeleteWorkerAsync(_workerServiceMock.Object, workerId);

        // Assert
        actual.Result.Should().BeOfType<NoContent>();

        _workerServiceMock.Verify(
            service => service.RemoveAsync(It.Is<Guid>(argument => argument.Equals(workerId))),
            Times.Once);
    }

    [Fact]
    public async Task DeleteWithInvlaidIdParameter_WhenCalled_Returns404NotFoundWithMessage()
    {
        // Arrange
        var workerId = Guid.NewGuid();

        string expectedMessage = $"Record with Id: {workerId} not found";

        _workerServiceMock.Setup(service => service.RemoveAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<NoContent, BadRequest<string>, ProblemHttpResult> actual =
            await WorkerEndpointsExtension.DeleteWorkerAsync(_workerServiceMock.Object, workerId);

        // Assert
        actual.Result.Should().BeOfType<BadRequest<string>>();
        actual.Result.As<BadRequest<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _workerServiceMock.Verify(
            service => service.RemoveAsync(It.Is<Guid>(argument => argument.Equals(workerId))),
            Times.Once);
    }
}

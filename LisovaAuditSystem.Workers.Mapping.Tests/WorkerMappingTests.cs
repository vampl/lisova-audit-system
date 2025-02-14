using FluentAssertions;

using LisovaAuditSystem.Workers.API.Common.Extensions.Mappings;
using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;
using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.Mapping.Tests;

public class WorkerMappingTests
{
    [Fact]
    public void ToDto_WhenCalledOnEntity_ReturnsDto()
    {
        // Arrange
        Worker worker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        WorkerDto expected =
            new(
                id: worker.Id,
                lastName: worker.LastName,
                name: worker.Name,
                birthDate: worker.BirthDate,
                phone: worker.Phone,
                email: worker.Email);

        // Act
        WorkerDto actual = worker.ToDto();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToEntity_WhenCalledOnDto_ReturnsEntity()
    {
        // Arrange
        WorkerDto worker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        Worker expected =
            new(
                id: worker.Id,
                lastName: worker.LastName,
                name: worker.Name,
                birthDate: worker.BirthDate,
                phone: worker.Phone,
                email: worker.Email);

        // Act
        Worker actual = worker.ToEntity();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToGetPayload_WhenCalledOnDto_ReturnsPayload()
    {
        // Arrange
        WorkerDto worker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        GetWorkerPayload expected =
            new(
                id: worker.Id,
                lastName: worker.LastName,
                name: worker.Name,
                birthDate: worker.BirthDate,
                phone: worker.Phone,
                email: worker.Email);

        // Act
        GetWorkerPayload actual = worker.ToPayload();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToDto_WhenCalledOnPostPayload_ReturnsDto()
    {
        // Arrange
        PostWorkerPayload postWorkerPayload =
            new(
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        WorkerDto expected =
            new(
                id: Guid.Empty,
                lastName: postWorkerPayload.LastName,
                name: postWorkerPayload.Name,
                birthDate: postWorkerPayload.BirthDate,
                phone: postWorkerPayload.Phone,
                email: postWorkerPayload.Email);

        // Act
        WorkerDto actual = postWorkerPayload.ToDto();

        // Assert
        actual.Should().BeEquivalentTo(expected, options => options.Excluding(worker => worker.Id));
    }

    [Fact]
    public void ToDto_WhenCalledOnPutPayload_ReturnsDto()
    {
        // Arrange
        PutWorkerPayload putWorkerPayload =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        WorkerDto expected =
            new(
                id: putWorkerPayload.Id,
                lastName: putWorkerPayload.LastName,
                name: putWorkerPayload.Name,
                birthDate: putWorkerPayload.BirthDate,
                phone: putWorkerPayload.Phone,
                email: putWorkerPayload.Email);

        // Act
        WorkerDto actual = putWorkerPayload.ToDto();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

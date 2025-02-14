using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;
using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.API.Common.Extensions.Mappings;

public static class WorkerMappingExtension
{
    public static WorkerDto ToDto(this Worker worker)
    {
        return new WorkerDto(
            id: worker.Id,
            lastName: worker.LastName,
            name: worker.Name,
            birthDate: worker.BirthDate,
            phone: worker.Phone,
            email: worker.Email);
    }

    public static WorkerDto ToDto(this PostWorkerPayload workerPayload)
    {
        return new WorkerDto(
            id: Guid.NewGuid(),
            lastName: workerPayload.LastName,
            name: workerPayload.Name,
            birthDate: workerPayload.BirthDate,
            phone: workerPayload.Phone,
            email: workerPayload.Email);
    }

    public static WorkerDto ToDto(this PutWorkerPayload workerPayload)
    {
        return new WorkerDto(
            id: workerPayload.Id,
            lastName: workerPayload.LastName,
            name: workerPayload.Name,
            birthDate: workerPayload.BirthDate,
            phone: workerPayload.Phone,
            email: workerPayload.Email);
    }

    public static Worker ToEntity(this WorkerDto worker)
    {
        return new Worker(
            id: worker.Id,
            lastName: worker.LastName,
            name: worker.Name,
            birthDate: worker.BirthDate,
            phone: worker.Phone,
            email: worker.Email);
    }

    public static GetWorkerPayload ToPayload(this WorkerDto workerDto)
    {
        return new GetWorkerPayload(
            id: workerDto.Id,
            lastName: workerDto.LastName,
            name: workerDto.Name,
            birthDate: workerDto.BirthDate,
            phone: workerDto.Phone,
            email: workerDto.Email);
    }
}

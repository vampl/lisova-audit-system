using System.Text.Json.Serialization;

using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.API.Models.Responses;

public class GetWorkersResponse(IReadOnlyList<GetWorkerPayload> workersPayloads)
{
    [JsonPropertyName("workers")]
    public IReadOnlyList<GetWorkerPayload> WorkersPayloads { get; set; } = workersPayloads;
}

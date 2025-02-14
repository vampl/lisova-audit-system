using System.Text.Json.Serialization;

using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.API.Models.Responses;

public class GetWorkerResponse(GetWorkerPayload workerPayload)
{
    [JsonPropertyName("worker")]
    public GetWorkerPayload WorkerPayload { get; set; } = workerPayload;
}

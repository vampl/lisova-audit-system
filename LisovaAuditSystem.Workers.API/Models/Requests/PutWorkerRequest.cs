using System.Text.Json.Serialization;

using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.API.Models.Requests;

public class PutWorkerRequest(PutWorkerPayload putWorkerPayload)
{
    [JsonPropertyName("worker")]
    public PutWorkerPayload PutWorkerPayload { get; set; } = putWorkerPayload;
}

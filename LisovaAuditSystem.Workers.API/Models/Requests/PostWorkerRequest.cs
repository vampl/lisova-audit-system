using System.Text.Json.Serialization;

using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.API.Models.Requests;

public class PostWorkerRequest(PostWorkerPayload postWorkerPayload)
{
    [JsonPropertyName("worker")]
    public PostWorkerPayload PostWorkerPayload { get; set; } = postWorkerPayload;
}

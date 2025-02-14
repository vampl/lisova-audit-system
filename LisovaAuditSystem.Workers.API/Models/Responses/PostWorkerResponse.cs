using System.Text.Json.Serialization;

namespace LisovaAuditSystem.Workers.API.Models.Responses;

public class PostWorkerResponse(Guid workerId)
{
    [JsonPropertyName("workerId")]
    public Guid WorkerId { get; set; } = workerId;
}

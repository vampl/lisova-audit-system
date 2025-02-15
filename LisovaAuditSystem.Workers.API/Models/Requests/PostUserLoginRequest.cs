using System.Text.Json.Serialization;

using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.API.Models.Requests;

public class PostUserLoginRequest(PostUserLoginPayload postUserLoginPayload)
{
    [JsonPropertyName("loginCredentials")]
    public PostUserLoginPayload PostUserLoginPayload { get; set; } = postUserLoginPayload;
}

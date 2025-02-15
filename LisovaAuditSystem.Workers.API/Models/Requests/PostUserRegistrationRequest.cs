using System.Text.Json.Serialization;

using LisovaAuditSystem.Workers.API.Models.Payloads;

namespace LisovaAuditSystem.Workers.API.Models.Requests;

public class PostUserRegistrationRequest(PostUserRegistrationPayload postUserRegistrationPayload)
{
    [JsonPropertyName("registrationCredentials")]
    public PostUserRegistrationPayload PostUserRegistrationPayload { get; set; } = postUserRegistrationPayload;
}

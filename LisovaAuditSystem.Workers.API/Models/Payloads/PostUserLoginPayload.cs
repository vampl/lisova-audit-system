using System.Text.Json.Serialization;

namespace LisovaAuditSystem.Workers.API.Models.Payloads;

public class PostUserLoginPayload(
    string email,
    string password)
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = email;

    [JsonPropertyName("password")]
    public string Password { get; set; } = password;
}

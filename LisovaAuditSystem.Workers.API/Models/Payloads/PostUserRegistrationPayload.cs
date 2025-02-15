using System.Text.Json.Serialization;

namespace LisovaAuditSystem.Workers.API.Models.Payloads;

public class PostUserRegistrationPayload(
    string userName,
    string email,
    string password)
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = userName;

    [JsonPropertyName("email")]
    public string Email { get; set; } = email;

    [JsonPropertyName("password")]
    public string Password { get; set; } = password;
}

using System.Text.Json.Serialization;

namespace LisovaAuditSystem.Workers.API.Models.Payloads;

public class PostWorkerPayload(
    string lastName,
    string name,
    DateTimeOffset birthDate,
    string phone,
    string? email)
{
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = lastName;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("birthDate")]
    public DateTimeOffset BirthDate { get; set; } = birthDate;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = phone;

    [JsonPropertyName("email")]
    public string? Email { get; set; } = email;
}

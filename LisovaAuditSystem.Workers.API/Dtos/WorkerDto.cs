using System.Text.RegularExpressions;

namespace LisovaAuditSystem.Workers.API.Dtos;

public partial class WorkerDto(
    Guid id,
    string lastName,
    string name,
    DateTimeOffset birthDate,
    string phone,
    string? email)
{
    public Guid Id { get; set; } = id;

    public string LastName { get; set; } = lastName;

    public string Name { get; set; } = name;

    public DateTimeOffset BirthDate { get; set; } = birthDate;

    public string Phone { get; set; } = phone;

    public string? Email { get; set; } = email;

    [GeneratedRegex(@"^\d{12}$")]
    private static partial Regex PhoneNumberRegexPattern();

    [GeneratedRegex(@"^[\w\-_.]+@[\w]+\.\w+(\.\w+)?$")]
    private static partial Regex EmailRegexPattern();

    public void ValidateAndThrow()
    {
        var errors = new List<string>();

        if (Id == Guid.Empty)
            errors.Add($"{nameof(Id)} cannot be empty.");

        var requiredFields =
            new Dictionary<string, string>
            {
                { nameof(LastName), LastName },
                { nameof(Name), Name },
                { nameof(Phone), Phone }
            };

        foreach ((string fieldName, string value) in requiredFields)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add($"{fieldName} cannot be null, empty, or whitespace.");
            }
        }

        if (BirthDate > DateTime.Now)
        {
            errors.Add($"{nameof(BirthDate)} cannot be in the future.");
        }

        if (!PhoneNumberRegexPattern().IsMatch(Phone))
        {
            errors.Add($"{nameof(Phone)} is not a valid phone format.");
        }

        if (Email is not null && !EmailRegexPattern().IsMatch(Email))
        {
            errors.Add($"{nameof(Email)} is not a valid email format.");
        }

        if (errors.Count != 0)
        {
            throw new InvalidOperationException($"Validation failed:\n{string.Join("\n", errors)}");
        }
    }
}

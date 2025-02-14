namespace LisovaAuditSystem.Workers.API.Entities;

public class Worker(
    Guid id,
    string lastName,
    string name,
    DateTimeOffset birthDate,
    string phone,
    string? email)
{
    public Guid Id { get; private set; } = id;

    public string LastName { get; private set; } = lastName;

    public string Name { get; private set; } = name;

    public DateTimeOffset BirthDate { get; private set; } = birthDate;

    public string Phone { get; private set; } = phone;

    public string? Email { get; private set; } = email;
}

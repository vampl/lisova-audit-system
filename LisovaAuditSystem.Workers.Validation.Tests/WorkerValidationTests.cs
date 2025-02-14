using FluentAssertions;

using LisovaAuditSystem.Workers.API.Dtos;

namespace LisovaAuditSystem.Workers.Validation.Tests;

public class WorkerValidationTests
{
    [Fact]
    public void ValidateAndThrow_WhenCalledOnValidInstance_DoesNotThrowAnyException()
    {
        // Arrange
        WorkerDto worker =
            new(
                id: Guid.NewGuid(),
                lastName: "Doe",
                name: "Joe",
                birthDate: DateTimeOffset.Now.AddYears(-20),
                phone: "380672052196",
                email: null);

        // Act & Assert
        FluentActions.Invoking(() => worker.ValidateAndThrow())
            .Should()
            .NotThrow<InvalidOperationException>();
    }

    [Fact]
    public void ValidateAndThrow_WhenCalledOnInvalidInstance_ThrowsInvalidOperationException()
    {// Arrange
        WorkerDto worker =
            new(
                id: Guid.Empty,
                lastName: "",
                name: "",
                birthDate: DateTimeOffset.Now.AddYears(+20),
                phone: "jdfhdp",
                email: "@mail.com@");

        // Act & Assert
        FluentActions.Invoking(() => worker.ValidateAndThrow())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*Id*LastName*Name*BirthDate*Phone*Email*");
    }
}

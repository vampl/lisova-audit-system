using System.Security.Cryptography;

using FluentAssertions;

using LisovaAuditSystem.Workers.API.Services;

namespace LisovaAuditSystem.Workers.Business.Tests.Services;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void Hash_WhenCalledWithPasswordString_ReturnsHashedPassword()
    {
        // Arrange
        string password = "password";

        // Act
        string actual = _passwordHasher.Hash(password);

        // Assert
        string[] parts = actual.Split('-');
        byte[] actualHash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);

        byte[] expectedHash =
            Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: 100000,
                hashAlgorithm: HashAlgorithmName.SHA512,
                outputLength: 32);

        actual.Should().NotBeNullOrEmpty();

        actualHash.Should().HaveCount(32);
        actualHash.Should().BeEquivalentTo(expectedHash);
    }

    [Fact]
    public void Verify_WhenCalledWithSamePassword_ReturnsTrue()
    {
        // Arrange
        string password = "password";
        string passwordHash = _passwordHasher.Hash(password);

        // Act
        bool actual =
            _passwordHasher.Verify(
                password: password,
                passwordHash: passwordHash);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Verify_WhenCalledWithSamePassword_ReturnsFalse()
    {
        // Arrange
        string password = "password";
        string passwordHash = _passwordHasher.Hash($"{password}-password");

        // Act
        bool actual =
            _passwordHasher.Verify(
                password: password,
                passwordHash: passwordHash);

        // Assert
        actual.Should().BeFalse();
    }
}

using System.Data.Common;

using FluentAssertions;

using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;
using LisovaAuditSystem.Workers.API.Interfaces.Services;
using LisovaAuditSystem.Workers.API.Services;

using Moq;

namespace LisovaAuditSystem.Workers.Business.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerationService> _jwtTokenGenerationServiceMock;

    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGenerationServiceMock = new Mock<IJwtTokenGenerationService>();

        _authenticationService =
            new AuthenticationService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenGenerationServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenCalledWithValidCredentials_ReturnsTokenString()
    {
        string hashedPassword = "hashedPassword";
        _passwordHasherMock.Setup(hasher => hasher.Hash(It.IsAny<string>()))
            .Returns(hashedPassword);

        UserRegisterCredentialsDto userCredentials =
            new(
                username: "JoeDoe",
                email: "joe_doe@gmail.com",
                password: "password");

        _userRepositoryMock.Setup(repository => repository.AddAsync(It.IsAny<UserDto>()))
            .Returns(Task.CompletedTask);

        string token = "potuspatienterducuntadazureusnix";
        _jwtTokenGenerationServiceMock.Setup(
                service => service.Generate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(token);

        // Act
        string actual = await _authenticationService.RegisterAsync(userCredentials);

        // Assert
        actual.Should().Be(token);
    }

    [Fact]
    public async Task LoginAsync_WhenCalledWithValidCredentials_ReturnsTokenString()
    {
        UserLoginCredentialsDto userCredentials =
            new(
                email: "joe_doe@gmail.com",
                password: "password");

        UserDto user =
            new(
                id: Guid.NewGuid(),
                userName: "JoeDoe",
                email: userCredentials.Email,
                passwordHash: "hashedPassword");
        _userRepositoryMock.Setup(repository => repository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        string token = "potuspatienterducuntadazureusnix";
        _jwtTokenGenerationServiceMock.Setup(
                service => service.Generate(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(token);

        // Act
        string actual = await _authenticationService.LoginAsync(userCredentials);

        // Assert
        actual.Should().Be(token);
    }


    [Fact]
    public async Task LoginAsync_WhenCalledWithInvalidEmailCredentials_ThrowsInvalidOperationException()
    {
        UserLoginCredentialsDto userCredentials =
            new(
                email: "joe_doe@gmail.com",
                password: "password");

        _userRepositoryMock.Setup(repository => repository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((UserDto?)null);

        // Act & Assert
        await FluentActions.Invoking(async () => await _authenticationService.LoginAsync(userCredentials))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task LoginAsync_WhenCalledWithInvalidPasswordCredentials_ThrowsInvalidOperationException()
    {
        UserLoginCredentialsDto userCredentials =
            new(
                email: "joe_doe@gmail.com",
                password: "password");

        UserDto user =
            new(
                id: Guid.NewGuid(),
                userName: "JoeDoe",
                email: userCredentials.Email,
                passwordHash: "hashedPassword");
        _userRepositoryMock.Setup(repository => repository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        // Act & Assert
        await FluentActions.Invoking(async () => await _authenticationService.LoginAsync(userCredentials))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }
}

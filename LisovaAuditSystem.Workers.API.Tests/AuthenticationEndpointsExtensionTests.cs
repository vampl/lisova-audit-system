using FluentAssertions;

using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Endpoints;
using LisovaAuditSystem.Workers.API.Interfaces.Services;
using LisovaAuditSystem.Workers.API.Models.Payloads;
using LisovaAuditSystem.Workers.API.Models.Requests;

using Microsoft.AspNetCore.Http.HttpResults;

using Moq;

namespace LisovaAuditSystem.Workers.API.Tests;

public class AuthenticationEndpointsExtensionTests
{
    private readonly Mock<IAuthenticationService> _authenticationServiceMock;

    public AuthenticationEndpointsExtensionTests()
    {
        _authenticationServiceMock = new Mock<IAuthenticationService>();
    }

    [Fact]
    public async Task RegisterAsync_WhenCalledWithValidRequestData_Returns200OkWithToken()
    {
        // Arrange
        PostUserRegistrationRequest request =
            new(
                new PostUserRegistrationPayload(
                    userName: "joe",
                    email: "joe@email.com",
                    password: "password"));

        string token = "NunquamDemittoExtum";

        _authenticationServiceMock.Setup(service => service.RegisterAsync(It.IsAny<UserRegisterCredentialsDto>()))
            .ReturnsAsync(token);

        // Act
        Results<Ok<string>, BadRequest<string>, ProblemHttpResult> actual =
            await AuthenticationEndpointsExtension.PostUserRegistration(_authenticationServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<Ok<string>>();
        actual.Result.As<Ok<string>>().Value.Should().BeEquivalentTo(token);

        _authenticationServiceMock.Verify(
            service =>
                service.RegisterAsync(
                    It.Is<UserRegisterCredentialsDto>(
                        argument =>
                            argument.UserName == "joe" &&
                            argument.Email == "joe@email.com" &&
                            argument.Password == "password")),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenCalledWithInvalidRequestUserNameData_Returns400BadRequestWithMessage()
    {
        // Arrange
        PostUserRegistrationRequest request =
            new(
                new PostUserRegistrationPayload(
                    userName: "joe",
                    email: "joe@email.com",
                    password: "password"));

        string expectedMessage = $"Record with username: {request.PostUserRegistrationPayload.UserName} already exists";

        _authenticationServiceMock.Setup(service => service.RegisterAsync(It.IsAny<UserRegisterCredentialsDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<Ok<string>, BadRequest<string>, ProblemHttpResult> actual =
            await AuthenticationEndpointsExtension.PostUserRegistration(_authenticationServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<BadRequest<string>>();
        actual.Result.As<BadRequest<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _authenticationServiceMock.Verify(
            service =>
                service.RegisterAsync(
                    It.Is<UserRegisterCredentialsDto>(
                        argument =>
                            argument.UserName == "joe" &&
                            argument.Email == "joe@email.com" &&
                            argument.Password == "password")),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenCalledWithInvalidRequestUserEmailData_Returns400BadRequestWithMessage()
    {
        // Arrange
        PostUserRegistrationRequest request =
            new(
                new PostUserRegistrationPayload(
                    userName: "joe",
                    email: "joe@email.com",
                    password: "password"));

        string expectedMessage = $"Record with username: {request.PostUserRegistrationPayload.Email} already exists";

        _authenticationServiceMock.Setup(service => service.RegisterAsync(It.IsAny<UserRegisterCredentialsDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<Ok<string>, BadRequest<string>, ProblemHttpResult> actual =
            await AuthenticationEndpointsExtension.PostUserRegistration(_authenticationServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<BadRequest<string>>();
        actual.Result.As<BadRequest<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _authenticationServiceMock.Verify(
            service =>
                service.RegisterAsync(
                    It.Is<UserRegisterCredentialsDto>(
                        argument =>
                            argument.UserName == "joe" &&
                            argument.Email == "joe@email.com" &&
                            argument.Password == "password")),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenCalledWithValidRequestData_Returns200OkWithToken()
    {
        // Arrange
        PostUserLoginRequest request =
            new(
                new PostUserLoginPayload(
                    email: "joe@email.com",
                    password: "password"));

        string token = "NunquamDemittoExtum";

        _authenticationServiceMock.Setup(service => service.LoginAsync(It.IsAny<UserLoginCredentialsDto>()))
            .ReturnsAsync(token);

        // Act
        Results<Ok<string>, BadRequest<string>, NotFound<string>, ProblemHttpResult> actual =
            await AuthenticationEndpointsExtension.PostUserLogin(_authenticationServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<Ok<string>>();
        actual.Result.As<Ok<string>>().Value.Should().BeEquivalentTo(token);

        _authenticationServiceMock.Verify(
            service =>
                service.LoginAsync(
                    It.Is<UserLoginCredentialsDto>(
                        argument =>
                            argument.Email == "joe@email.com" &&
                            argument.Password == "password")),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenCalledWithInvalidRequestData_Returns404NotFoundWithMessage()
    {
        // Arrange
        PostUserLoginRequest request =
            new(
                new PostUserLoginPayload(
                    email: "joe@email.com",
                    password: "password"));

        string expectedMessage = $"Record with email: {request.PostUserLoginPayload.Email} not found";

        _authenticationServiceMock.Setup(service => service.LoginAsync(It.IsAny<UserLoginCredentialsDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<Ok<string>, BadRequest<string>, NotFound<string>, ProblemHttpResult> actual =
            await AuthenticationEndpointsExtension.PostUserLogin(_authenticationServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<NotFound<string>>();
        actual.Result.As<NotFound<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _authenticationServiceMock.Verify(
            service =>
                service.LoginAsync(
                    It.Is<UserLoginCredentialsDto>(
                        argument =>
                            argument.Email == "joe@email.com" &&
                            argument.Password == "password")),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenCalledWithInvalidRequestData_Returns400BadRequestFoundWithMessage()
    {
        // Arrange
        PostUserLoginRequest request =
            new(
                new PostUserLoginPayload(
                    email: "joe@email.com",
                    password: "password"));

        string expectedMessage = "User password wrong";

        _authenticationServiceMock.Setup(service => service.LoginAsync(It.IsAny<UserLoginCredentialsDto>()))
            .ThrowsAsync(new InvalidOperationException(expectedMessage));

        // Act
        Results<Ok<string>, BadRequest<string>, NotFound<string>, ProblemHttpResult> actual =
            await AuthenticationEndpointsExtension.PostUserLogin(_authenticationServiceMock.Object, request);

        // Assert
        actual.Result.Should().BeOfType<BadRequest<string>>();
        actual.Result.As<BadRequest<string>>().Value.Should().BeEquivalentTo(expectedMessage);

        _authenticationServiceMock.Verify(
            service =>
                service.LoginAsync(
                    It.Is<UserLoginCredentialsDto>(
                        argument =>
                            argument.Email == "joe@email.com" &&
                            argument.Password == "password")),
            Times.Once);
    }
}

using FluentAssertions;

using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;
using LisovaAuditSystem.Workers.API.Infrastructure;
using LisovaAuditSystem.Workers.API.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LisovaAuditSystem.Workers.Data.Tests.Infrastructure.Repository;

public class UserRepositoryTests
{
    private readonly WorkersContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        DbContextOptions<WorkersContext> options =
            new DbContextOptionsBuilder<WorkersContext>()
                .UseInMemoryDatabase(databaseName: $"Database-{Guid.NewGuid()}")
                .ConfigureWarnings(config => config.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

        _context = new WorkersContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsAllUsers()
    {
        // Arrange
        IList<User> users =
        [
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365"),
            new(
                id: Guid.NewGuid(),
                userName: "jean_doe",
                email: "jean.doe@gmail.com",
                passwordHash: "F498371A-3EB5-4ECA-B8D1-AA22D5FDA915")
        ];

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        IList<UserDto> expected =
            users.Select(
                    user =>
                        new UserDto(
                            id: user.Id,
                            userName: user.UserName,
                            email: user.Email,
                            passwordHash: user.PasswordHash))
                .ToList();

        // Act
        IList<UserDto> actual = await _repository.GetAllAsync();

        // Assert
        actual.Should().NotBeEmpty().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsEmptyCollection()
    {
        // Arrange & Act
        IList<UserDto> actual = await _repository.GetAllAsync();

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalledWithValidId_ReturnsUser()
    {
        // Arrange
        var id = Guid.NewGuid();

        User user =
            new(
                id: id,
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        UserDto expected =
            new(
                id: user.Id,
                userName: user.UserName,
                email: user.Email,
                passwordHash: user.PasswordHash);

        // Act
        UserDto? actual = await _repository.GetByIdAsync(id);

        // Assert
        actual.Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalledWithInvalidId_ReturnsNull()
    {
        // Arrange & Act
        UserDto? actual = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCalledWithValidData_AddsUser()
    {
        // Arrange
        UserDto addUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        User expected =
            new(
                id: addUser.Id,
                userName: addUser.UserName,
                email: addUser.Email,
                passwordHash: addUser.PasswordHash);

        // Act
        await _repository.AddAsync(addUser);

        // Assert
        User? actual = await _context.Users.FirstOrDefaultAsync(user => user.Id == addUser.Id);

        actual.Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task AddAsync_WhenCalledWithInvalidIdData_ThrowsInvalidOperationException()
    {
        // Arrange
        User originalUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(originalUser);
        await _context.SaveChangesAsync();

        UserDto existingUser =
            new(
                id: originalUser.Id,
                userName: originalUser.UserName,
                email: originalUser.Email,
                passwordHash: originalUser.PasswordHash);

        // Act & Assert
        await FluentActions.Invoking(() => _repository.AddAsync(existingUser))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }


    [Fact]
    public async Task AddAsync_WhenCalledWithInvalidUserNameData_ThrowsInvalidOperationException()
    {
        // Arrange
        User originalUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(originalUser);
        await _context.SaveChangesAsync();

        UserDto existingUser =
            new(
                id: Guid.NewGuid(),
                userName: originalUser.UserName,
                email: originalUser.Email,
                passwordHash: originalUser.PasswordHash);

        // Act & Assert
        await FluentActions.Invoking(() => _repository.AddAsync(existingUser))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }


    [Fact]
    public async Task AddAsync_WhenCalledWithInvalidEmailData_ThrowsInvalidOperationException()
    {
        // Arrange
        User originalUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(originalUser);
        await _context.SaveChangesAsync();

        UserDto existingUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe_79",
                email: originalUser.Email,
                passwordHash: originalUser.PasswordHash);

        // Act & Assert
        await FluentActions.Invoking(() => _repository.AddAsync(existingUser))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateAsync_WhenCalledWithValidData_UpdatesUser()
    {
        // Arrange
        User originalUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(originalUser);
        await _context.SaveChangesAsync();

        UserDto updateUser =
            new(
                id: originalUser.Id,
                userName: "John",
                email: originalUser.Email,
                passwordHash: originalUser.PasswordHash);

        User expected =
            new(
                id: updateUser.Id,
                userName: updateUser.UserName,
                email: updateUser.Email,
                passwordHash: updateUser.PasswordHash);

        // Act
        await _repository.UpdateAsync(updateUser);

        // Assert
        User? actual = await _context.Users.FirstOrDefaultAsync(user => user.Id == updateUser.Id);

        actual.Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateAsync_WhenCalledWithInvalidData_ThrowsInvalidOperationException()
    {
        // Arrange
        User originalUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(originalUser);
        await _context.SaveChangesAsync();

        UserDto nonExistingUser =
            new(
                id: Guid.NewGuid(),
                userName: originalUser.UserName,
                email: originalUser.Email,
                passwordHash: originalUser.PasswordHash);

        // Act & Assert
        await FluentActions.Invoking(() => _repository.UpdateAsync(nonExistingUser))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalledWithValidId_DeletesUser()
    {
        // Arrange
        User originalUser =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(originalUser);
        await _context.SaveChangesAsync();

        Guid deleteId = originalUser.Id;

        // Act
        await _repository.DeleteAsync(deleteId);

        // Assert
        User? actual = await _context.Users.FirstOrDefaultAsync(user => user.Id == deleteId);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalledWithInvalidId_ThrowsException()
    {
        // Arrange
        var nonExistingUserId = Guid.NewGuid();

        // Act & Assert
        await FluentActions.Invoking(() => _repository.DeleteAsync(nonExistingUserId))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }


    [Fact]
    public async Task GetByEmailAsync_WhenCalledWithValidEmail_ReturnsUser()
    {
        // Arrange
        string email = "joe.doe@gmail.com";

        User user =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: email,
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        UserDto expected =
            new(
                id: user.Id,
                userName: user.UserName,
                email: user.Email,
                passwordHash: user.PasswordHash);

        // Act
        UserDto? actual = await _repository.GetByEmailAsync(email);

        // Assert
        actual.Should().NotBeNull().And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenCalledWithInvalidEmail_ReturnsNull()
    {
        // Arrange & Act
        UserDto? actual = await _repository.GetByEmailAsync("email@email.com");

        // Assert
        actual.Should().BeNull();
    }

}

using FluentAssertions;

using LisovaAuditSystem.Workers.API.Common.Extensions.Mappings;
using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;

namespace LisovaAuditSystem.Workers.Mapping.Tests;

public class UserMappingTests
{
    [Fact]
    public void ToDto_WhenCalledOnEntity_ReturnsDto()
    {
        // Arrange
        User user =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        UserDto expected =
            new(
                id: user.Id,
                userName: user.UserName,
                email: user.Email,
                passwordHash: user.PasswordHash);

        // Act
        UserDto actual = user.ToDto();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToEntity_WhenCalledOnDto_ReturnsEntity()
    {
        // Arrange
        UserDto user =
            new(
                id: Guid.NewGuid(),
                userName: "Joe_Doe",
                email: "joe.doe@gmail.com",
                passwordHash: "47D12392-B623-4942-B919-4E713BB4F365");

        User expected =
            new(
                id: user.Id,
                userName: user.UserName,
                email: user.Email,
                passwordHash: user.PasswordHash);

        // Act
        User actual = user.ToEntity();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

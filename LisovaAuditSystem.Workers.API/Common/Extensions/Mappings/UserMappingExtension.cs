using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Entities;

namespace LisovaAuditSystem.Workers.API.Common.Extensions.Mappings;

public static class UserMappingExtension
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            id: user.Id,
            userName: user.UserName,
            email: user.Email,
            passwordHash: user.PasswordHash);
    }

    public static User ToEntity(this UserDto user)
    {
        return new User(
            id: user.Id,
            userName: user.UserName,
            email: user.Email,
            passwordHash: user.PasswordHash);
    }
}

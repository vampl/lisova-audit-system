using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using LisovaAuditSystem.Workers.API.Common.Configurations;
using LisovaAuditSystem.Workers.API.Interfaces.Infrastructure.Repository;

using Microsoft.IdentityModel.Tokens;

namespace LisovaAuditSystem.Workers.API.Services;

public class JwtTokenGenerationService(JwtConfiguration configuration) : IJwtTokenGenerationService
{
    public string Generate(Guid userId, string userName, string userEmail)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtRegisteredClaimNames.Email, userEmail)
        ];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration.Secret));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token =
            new(
                issuer: configuration.Issuer,
                audience: configuration.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(configuration.ExpireDays),
                signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

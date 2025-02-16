using System.IdentityModel.Tokens.Jwt;

using FluentAssertions;

using LisovaAuditSystem.Workers.API.Common.Configurations;
using LisovaAuditSystem.Workers.API.Services;

using Microsoft.Extensions.Configuration;

using Moq;

namespace LisovaAuditSystem.Workers.Business.Tests.Services;

public class JwtTokenGenerationServiceTests
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly JwtTokenGenerationService _jwtTokenGenerationService;

    public JwtTokenGenerationServiceTests()
    {
        string issuer = "http://issuer:5000";
        string audience = "http://audience:5000";
        string expireDays = "7";
        string secret = "CDA18035-2E47-4A97-B063-98EEEE941436";

        Mock<IConfigurationSection> sectionMock = new();
        Mock<IConfiguration> configurationMock = new();

        sectionMock.Setup(section => section[nameof(JwtConfiguration.Issuer)])
            .Returns(issuer);
        sectionMock.Setup(section => section[nameof(JwtConfiguration.Audience)])
            .Returns(audience);
        sectionMock.Setup(section => section[nameof(JwtConfiguration.ExpireDays)])
            .Returns(expireDays);
        sectionMock.Setup(section => section[nameof(JwtConfiguration.Secret)])
            .Returns(secret);

        configurationMock.Setup(configuration => configuration.GetSection("JwtConfiguration"))
            .Returns(sectionMock.Object);

        _jwtConfiguration = new JwtConfiguration(configurationMock.Object);
        _jwtTokenGenerationService = new JwtTokenGenerationService(_jwtConfiguration);
    }

    [Fact]
    public void Generate_WhenCalled_ReturnsTokenContainingConfigurationDataAndClaims()
    {
        var userId = Guid.NewGuid();
        string userName = "testUser";
        string userEmail = "test@example.com";

        // Act
        string actual = _jwtTokenGenerationService.Generate(userId, userName, userEmail);

        // Assert
        JwtSecurityTokenHandler tokenHandler = new();
        var actualToken = tokenHandler.ReadToken(actual) as JwtSecurityToken;

        actualToken.Should().NotBeNull();
        actualToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub)
            .Value
            .Should()
            .Be(userId.ToString());
        actualToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.UniqueName)
            .Value
            .Should()
            .Be(userName);
        actualToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Email)
            .Value
            .Should()
            .Be(userEmail);
        actualToken.Issuer.Should().Be(_jwtConfiguration.Issuer);
        actualToken.Audiences.First().Should().Be(_jwtConfiguration.Audience);
        actualToken.ValidTo.Should()
            .BeCloseTo(DateTime.UtcNow.AddDays(_jwtConfiguration.ExpireDays), TimeSpan.FromDays(1));
    }
}

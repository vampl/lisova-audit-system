using System.Globalization;

namespace LisovaAuditSystem.Workers.API.Common.Configurations;

public class JwtConfiguration
{
    public JwtConfiguration(IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("JwtConfiguration");

        Issuer = section[nameof(Issuer)] ?? string.Empty;
        Audience = section[nameof(Audience)] ?? string.Empty;
        Secret = section[nameof(Secret)] ?? string.Empty;
        ExpireDays = int.Parse(section[nameof(ExpireDays)] ?? string.Empty, CultureInfo.InvariantCulture);
    }

    public string Issuer { get; }

    public string Audience { get; }

    public string Secret { get; }

    public int ExpireDays { get; }
}

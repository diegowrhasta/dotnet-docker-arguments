using web_api_override.Services.Configurations;

namespace web_api_override.Services;

public class ScoopsConfiguration : IScoopsConfiguration
{
    public ConfigurationModel? Config { get; set; }

    public ScoopsConfiguration(IConfiguration configuration)
    {
        Config = configuration
            .GetSection("BigScoops")
            .Get<ConfigurationModel>();
    }
}
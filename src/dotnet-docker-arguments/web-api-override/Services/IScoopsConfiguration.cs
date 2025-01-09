using web_api_override.Services.Configurations;

namespace web_api_override.Services;

public interface IScoopsConfiguration
{
    ConfigurationModel? Config { get; set; }
}
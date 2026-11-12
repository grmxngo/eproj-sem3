namespace HeritageTrail.Web.Services;

/// <summary>Bound from Content/config/environments.json (SSOT for staging/prod split).</summary>
public sealed record EnvironmentConfig
{
    public int AppPort { get; init; }
    public int MongoPort { get; init; }
    public string Database { get; init; } = "";
    public string DbPath { get; init; } = "";
    public string Uploads { get; init; } = "";
    public string MailOutbox { get; init; } = "";
    public string SeedMode { get; init; } = "Reset";
    public string? Badge { get; init; }
    public bool AtomsGallery { get; init; }
}

public static class EnvironmentNames
{
    public const string Staging = "Staging";
    public const string Production = "Production";
}

public sealed class EnvironmentInfo
{
    public EnvironmentInfo(IWebHostEnvironment host, EnvironmentConfig config)
    {
        Name = host.EnvironmentName;
        Config = config;
    }

    public string Name { get; }
    public EnvironmentConfig Config { get; }
    public bool IsStaging => string.Equals(Name, EnvironmentNames.Staging, StringComparison.OrdinalIgnoreCase);
    public string? Badge => Config.Badge;
    public bool IsAtomsEnabled => Config.AtomsGallery && IsStaging;
}

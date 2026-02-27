namespace MiniTasksSystem.Api.Settings;

public sealed class CorsOptions
{
    public const string PolicyName = "CorsPolicy";
    public const string SectionName = "Cors";

    public required string[] AllowedOrigins { get; init; }
}

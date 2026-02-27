namespace MiniTasksSystem.Api.Settings;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public required string ServiceName { get; init; }
    public required string Endpoint { get; init; }
}

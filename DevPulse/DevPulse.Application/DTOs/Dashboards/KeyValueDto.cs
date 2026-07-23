namespace DevPulse.Application.DTOs.Dashboards;

public sealed record KeyValueDto
{
    public string Label { get; init; } = default!;

    public long Value { get; init; }
}
namespace EnvCheck.Core;

public sealed class EnvComparisonResult
{
    public required IReadOnlyList<string> MissingKeys { get; init; }
    public required IReadOnlyList<string> ExtraKeys { get; init; }
    public required IReadOnlyList<string> EmptyValueKeys { get; init; }

    public bool HasErrors => MissingKeys.Count > 0 || EmptyValueKeys.Count > 0;
}

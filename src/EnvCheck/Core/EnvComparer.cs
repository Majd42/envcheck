namespace EnvCheck.Core;

public static class EnvComparer
{
    public static EnvComparisonResult Compare(EnvFile example, EnvFile actual)
    {
        var missingKeys = new List<string>();
        var emptyValueKeys = new List<string>();

        foreach (var key in example.Keys)
        {
            if (!actual.TryGetValue(key, out var value))
            {
                missingKeys.Add(key);
                continue;
            }

            if (string.IsNullOrEmpty(value))
                emptyValueKeys.Add(key);
        }

        var extraKeys = actual.Keys
            .Where(key => !example.ContainsKey(key))
            .ToList();

        return new EnvComparisonResult
        {
            MissingKeys = missingKeys,
            ExtraKeys = extraKeys,
            EmptyValueKeys = emptyValueKeys
        };
    }
}

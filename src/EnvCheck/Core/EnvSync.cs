using System.Text;

namespace EnvCheck.Core;

public static class EnvSync
{
    public static IReadOnlyList<string> AppendMissingKeys(string envPath, EnvFile example, IReadOnlyList<string> missingKeys)
    {
        if (missingKeys.Count == 0)
            return [];

        var existing = File.Exists(envPath) ? File.ReadAllText(envPath) : string.Empty;
        var builder = new StringBuilder();

        if (existing.Length > 0 && !existing.EndsWith('\n'))
            builder.AppendLine();

        var added = new List<string>();
        foreach (var key in missingKeys)
        {
            example.TryGetValue(key, out var value);
            builder.AppendLine($"{key}={value}");
            added.Add(key);
        }

        File.AppendAllText(envPath, builder.ToString());
        return added;
    }
}

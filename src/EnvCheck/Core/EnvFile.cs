namespace EnvCheck.Core;

public sealed class EnvFile
{
    private readonly Dictionary<string, string> _values;
    private readonly List<string> _keysInOrder;

    private EnvFile(List<string> keysInOrder, Dictionary<string, string> values)
    {
        _keysInOrder = keysInOrder;
        _values = values;
    }

    public IReadOnlyList<string> Keys => _keysInOrder;

    public bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value!);

    public bool ContainsKey(string key) => _values.ContainsKey(key);

    public static EnvFile Parse(IEnumerable<string> lines)
    {
        var keysInOrder = new List<string>();
        var values = new Dictionary<string, string>();

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            if (line.StartsWith("export ", StringComparison.Ordinal))
                line = line["export ".Length..].TrimStart();

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
                continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            value = StripInlineComment(value);
            value = StripQuotes(value);

            if (!values.ContainsKey(key))
                keysInOrder.Add(key);

            values[key] = value;
        }

        return new EnvFile(keysInOrder, values);
    }

    public static EnvFile ParseFile(string path) => Parse(File.ReadAllLines(path));

    private static string StripQuotes(string value)
    {
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            return value[1..^1];

        if (value.Length >= 2 && value[0] == '\'' && value[^1] == '\'')
            return value[1..^1];

        return value;
    }

    private static string StripInlineComment(string value)
    {
        if (value.Length > 0 && (value[0] == '"' || value[0] == '\''))
            return value;

        var hashIndex = value.IndexOf('#');
        return hashIndex < 0 ? value : value[..hashIndex].TrimEnd();
    }
}

namespace EnvCheck.Cli;

public sealed class CliOptions
{
    public string ExamplePath { get; init; } = ".env.example";
    public string EnvPath { get; init; } = ".env";
    public bool Strict { get; init; }
    public bool Fix { get; init; }
    public bool Json { get; init; }
    public bool ShowHelp { get; init; }

    public static CliOptions? Parse(string[] args, out string? error)
    {
        error = null;
        var examplePath = ".env.example";
        var envPath = ".env";
        var strict = false;
        var fix = false;
        var json = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-h":
                case "--help":
                    return new CliOptions { ShowHelp = true };

                case "--strict":
                    strict = true;
                    break;

                case "--fix":
                    fix = true;
                    break;

                case "--json":
                    json = true;
                    break;

                case "--example":
                    if (!TryReadValue(args, ref i, out examplePath, out error))
                        return null;
                    break;

                case "--env":
                    if (!TryReadValue(args, ref i, out envPath, out error))
                        return null;
                    break;

                default:
                    error = $"Unknown argument: {args[i]}";
                    return null;
            }
        }

        return new CliOptions
        {
            ExamplePath = examplePath,
            EnvPath = envPath,
            Strict = strict,
            Fix = fix,
            Json = json
        };
    }

    private static bool TryReadValue(string[] args, ref int i, out string value, out string? error)
    {
        if (i + 1 >= args.Length)
        {
            value = string.Empty;
            error = $"Missing value for argument: {args[i]}";
            return false;
        }

        value = args[++i];
        error = null;
        return true;
    }

    public const string HelpText = """
        envcheck - validate a .env file against a .env.example template

        Usage:
          envcheck [options]

        Options:
          --example <path>   Path to the template file (default: .env.example)
          --env <path>       Path to the file to validate (default: .env)
          --strict           Also fail when the env file has keys not present in the example
          --fix              Append missing keys to the env file, using the example's values
          --json             Output the comparison result as JSON (machine-readable)
          -h, --help         Show this help message

        Exit codes:
          0   env file matches the example
          1   env file is missing keys, has empty values, or (with --strict) extra keys
          2   usage error, e.g. a file could not be found
        """;
}

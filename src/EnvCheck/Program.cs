using EnvCheck.Cli;
using EnvCheck.Core;

var options = CliOptions.Parse(args, out var error);

if (error is not null)
{
    Console.Error.WriteLine($"Error: {error}");
    Console.Error.WriteLine();
    Console.Error.WriteLine(CliOptions.HelpText);
    return 2;
}

if (options!.ShowHelp)
{
    Console.WriteLine(CliOptions.HelpText);
    return 0;
}

if (!File.Exists(options.ExamplePath))
{
    Console.Error.WriteLine($"Error: example file not found: {options.ExamplePath}");
    return 2;
}

if (!File.Exists(options.EnvPath))
{
    Console.Error.WriteLine($"Error: env file not found: {options.EnvPath}");
    return 2;
}

var example = EnvFile.ParseFile(options.ExamplePath);
var actual = EnvFile.ParseFile(options.EnvPath);
var result = EnvComparer.Compare(example, actual);

if (options.Fix && result.MissingKeys.Count > 0)
{
    var added = EnvSync.AppendMissingKeys(options.EnvPath, example, result.MissingKeys);

    WriteColored($"Added {added.Count} missing key(s) to {options.EnvPath}:", ConsoleColor.Cyan);
    foreach (var key in added)
        WriteColored($"  - {key}", ConsoleColor.Cyan);
    Console.WriteLine();

    actual = EnvFile.ParseFile(options.EnvPath);
    result = EnvComparer.Compare(example, actual);
}

PrintKeyList($"Missing keys (present in {options.ExamplePath}, missing from {options.EnvPath}):", result.MissingKeys, ConsoleColor.Red);
PrintKeyList($"Empty values (present in {options.EnvPath} but blank):", result.EmptyValueKeys, ConsoleColor.Yellow);
PrintKeyList($"Extra keys (present in {options.EnvPath}, not in {options.ExamplePath}):", result.ExtraKeys, options.Strict ? ConsoleColor.Red : ConsoleColor.DarkGray);

var failed = result.HasErrors || (options.Strict && result.ExtraKeys.Count > 0);

if (!failed)
{
    WriteColored($"{options.EnvPath} matches {options.ExamplePath}", ConsoleColor.Green);
    return 0;
}

return 1;

static void PrintKeyList(string header, IReadOnlyList<string> keys, ConsoleColor color)
{
    if (keys.Count == 0)
        return;

    WriteColored(header, color);
    foreach (var key in keys)
        WriteColored($"  - {key}", color);
}

static void WriteColored(string text, ConsoleColor color)
{
    var previous = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ForegroundColor = previous;
}

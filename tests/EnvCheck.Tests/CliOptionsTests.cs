using EnvCheck.Cli;
using Xunit;

namespace EnvCheck.Tests;

public class CliOptionsTests
{
    [Fact]
    public void Parse_UsesDefaults_WhenNoArgsGiven()
    {
        var options = CliOptions.Parse([], out var error);

        Assert.Null(error);
        Assert.NotNull(options);
        Assert.Equal(".env.example", options!.ExamplePath);
        Assert.Equal(".env", options.EnvPath);
        Assert.False(options.Strict);
        Assert.False(options.Fix);
        Assert.False(options.ShowHelp);
    }

    [Fact]
    public void Parse_ReadsCustomPathsAndStrictFlag()
    {
        var options = CliOptions.Parse(
            ["--example", "config/.env.example", "--env", "config/.env", "--strict"],
            out var error);

        Assert.Null(error);
        Assert.NotNull(options);
        Assert.Equal("config/.env.example", options!.ExamplePath);
        Assert.Equal("config/.env", options.EnvPath);
        Assert.True(options.Strict);
    }

    [Fact]
    public void Parse_ReadsFixFlag()
    {
        var options = CliOptions.Parse(["--fix"], out var error);

        Assert.Null(error);
        Assert.NotNull(options);
        Assert.True(options!.Fix);
    }

    [Fact]
    public void Parse_ReadsJsonFlag()
    {
        var options = CliOptions.Parse(["--json"], out var error);

        Assert.Null(error);
        Assert.NotNull(options);
        Assert.True(options!.Json);
    }

    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    public void Parse_SetsShowHelp(string helpFlag)
    {
        var options = CliOptions.Parse([helpFlag], out var error);

        Assert.Null(error);
        Assert.NotNull(options);
        Assert.True(options!.ShowHelp);
    }

    [Theory]
    [InlineData("-v")]
    [InlineData("--version")]
    public void Parse_SetsShowVersion(string versionFlag)
    {
        var options = CliOptions.Parse([versionFlag], out var error);

        Assert.Null(error);
        Assert.NotNull(options);
        Assert.True(options!.ShowVersion);
    }

    [Fact]
    public void Parse_ReturnsError_ForUnknownArgument()
    {
        var options = CliOptions.Parse(["--nope"], out var error);

        Assert.Null(options);
        Assert.Equal("Unknown argument: --nope", error);
    }

    [Fact]
    public void Parse_ReturnsError_WhenValueIsMissing()
    {
        var options = CliOptions.Parse(["--example"], out var error);

        Assert.Null(options);
        Assert.Equal("Missing value for argument: --example", error);
    }
}

using EnvCheck.Core;
using Xunit;

namespace EnvCheck.Tests;

public class EnvComparerTests
{
    [Fact]
    public void Compare_ReturnsNoErrors_WhenFilesMatch()
    {
        var example = EnvFile.Parse(["FOO=", "BAR="]);
        var actual = EnvFile.Parse(["FOO=1", "BAR=2"]);

        var result = EnvComparer.Compare(example, actual);

        Assert.Empty(result.MissingKeys);
        Assert.Empty(result.EmptyValueKeys);
        Assert.Empty(result.ExtraKeys);
        Assert.False(result.HasErrors);
    }

    [Fact]
    public void Compare_DetectsMissingKeys()
    {
        var example = EnvFile.Parse(["FOO=", "BAR="]);
        var actual = EnvFile.Parse(["FOO=1"]);

        var result = EnvComparer.Compare(example, actual);

        Assert.Equal(["BAR"], result.MissingKeys);
        Assert.True(result.HasErrors);
    }

    [Fact]
    public void Compare_DetectsEmptyValues()
    {
        var example = EnvFile.Parse(["FOO="]);
        var actual = EnvFile.Parse(["FOO="]);

        var result = EnvComparer.Compare(example, actual);

        Assert.Equal(["FOO"], result.EmptyValueKeys);
        Assert.True(result.HasErrors);
    }

    [Fact]
    public void Compare_DetectsExtraKeys_ButDoesNotCountAsErrorByDefault()
    {
        var example = EnvFile.Parse(["FOO="]);
        var actual = EnvFile.Parse(["FOO=1", "EXTRA=2"]);

        var result = EnvComparer.Compare(example, actual);

        Assert.Equal(["EXTRA"], result.ExtraKeys);
        Assert.False(result.HasErrors);
    }
}

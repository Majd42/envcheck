using EnvCheck.Core;
using Xunit;

namespace EnvCheck.Tests;

public class EnvFileTests
{
    [Fact]
    public void Parse_ReadsSimpleKeyValuePairs()
    {
        var env = EnvFile.Parse(["FOO=1", "BAR=2"]);

        Assert.True(env.TryGetValue("FOO", out var foo));
        Assert.Equal("1", foo);
        Assert.True(env.TryGetValue("BAR", out var bar));
        Assert.Equal("2", bar);
    }

    [Fact]
    public void Parse_SkipsBlankLinesAndComments()
    {
        var env = EnvFile.Parse(["", "# a comment", "FOO=1", "   ", "# FOO=ignored"]);

        Assert.Equal(["FOO"], env.Keys);
    }

    [Fact]
    public void Parse_StripsSurroundingQuotes()
    {
        var env = EnvFile.Parse(["FOO=\"hello world\"", "BAR='single quoted'"]);

        Assert.True(env.TryGetValue("FOO", out var foo));
        Assert.Equal("hello world", foo);
        Assert.True(env.TryGetValue("BAR", out var bar));
        Assert.Equal("single quoted", bar);
    }

    [Fact]
    public void Parse_StripsInlineCommentsOnUnquotedValues()
    {
        var env = EnvFile.Parse(["FOO=1 # trailing comment"]);

        Assert.True(env.TryGetValue("FOO", out var foo));
        Assert.Equal("1", foo);
    }

    [Fact]
    public void Parse_KeepsHashInsideQuotedValues()
    {
        var env = EnvFile.Parse(["FOO=\"value#withhash\""]);

        Assert.True(env.TryGetValue("FOO", out var foo));
        Assert.Equal("value#withhash", foo);
    }

    [Fact]
    public void Parse_HandlesExportPrefix()
    {
        var env = EnvFile.Parse(["export FOO=1"]);

        Assert.True(env.TryGetValue("FOO", out var foo));
        Assert.Equal("1", foo);
    }

    [Fact]
    public void Parse_LastValueWinsForDuplicateKeys()
    {
        var env = EnvFile.Parse(["FOO=1", "FOO=2"]);

        Assert.True(env.TryGetValue("FOO", out var foo));
        Assert.Equal("2", foo);
        Assert.Equal(["FOO"], env.Keys);
    }

    [Fact]
    public void Parse_TreatsEmptyValueAsEmptyString()
    {
        var env = EnvFile.Parse(["FOO="]);

        Assert.True(env.TryGetValue("FOO", out var foo));
        Assert.Equal(string.Empty, foo);
    }

    [Fact]
    public void Parse_IgnoresLinesWithoutEqualsSign()
    {
        var env = EnvFile.Parse(["not a valid line", "FOO=1"]);

        Assert.Equal(["FOO"], env.Keys);
    }
}

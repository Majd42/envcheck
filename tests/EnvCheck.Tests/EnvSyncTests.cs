using EnvCheck.Core;
using Xunit;

namespace EnvCheck.Tests;

public class EnvSyncTests
{
    [Fact]
    public void AppendMissingKeys_AppendsKeysWithExampleValues()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "FOO=1\n");
            var example = EnvFile.Parse(["FOO=", "BAR=default", "BAZ="]);

            var added = EnvSync.AppendMissingKeys(path, example, ["BAR", "BAZ"]);

            Assert.Equal(["BAR", "BAZ"], added);

            var updated = EnvFile.ParseFile(path);
            Assert.True(updated.TryGetValue("FOO", out var foo));
            Assert.Equal("1", foo);
            Assert.True(updated.TryGetValue("BAR", out var bar));
            Assert.Equal("default", bar);
            Assert.True(updated.TryGetValue("BAZ", out var baz));
            Assert.Equal(string.Empty, baz);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void AppendMissingKeys_AddsNewlineBeforeAppending_WhenFileHasNoTrailingNewline()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "FOO=1");
            var example = EnvFile.Parse(["FOO=", "BAR=2"]);

            EnvSync.AppendMissingKeys(path, example, ["BAR"]);

            var updated = EnvFile.ParseFile(path);
            Assert.Equal(["FOO", "BAR"], updated.Keys);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void AppendMissingKeys_DoesNothing_WhenNoMissingKeys()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "FOO=1\n");
            var before = File.ReadAllText(path);
            var example = EnvFile.Parse(["FOO="]);

            var added = EnvSync.AppendMissingKeys(path, example, []);

            Assert.Empty(added);
            Assert.Equal(before, File.ReadAllText(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}

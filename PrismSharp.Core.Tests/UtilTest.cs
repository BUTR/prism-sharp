using System.Text.RegularExpressions;
using Xunit;

namespace PrismSharp.Core.Tests;

public class UtilTest
{
    [Fact]
    public void MatchPattern_lookbehind_eq_True_Ok()
    {
        var regex = new Regex(@"see (chapter \d+(\.\d)*)", RegexOptions.IgnoreCase);
        var text = @"For more information, see Chapter 3.4.5.1";
        var match = Util.MatchPattern(regex, 0, text, true);
        Assert.NotNull(match);
        Assert.Equal(3, match.Groups.Length);
        Assert.Equal(".5.1", match.Groups[0]);
        Assert.Equal("Chapter 3.4.5.1", match.Groups[1]);
        Assert.Equal(".1", match.Groups[2]);
        Assert.Equal(37, match.Index);
    }

    [Theory]
    [InlineData("abc", 0, "abc")]
    [InlineData("abc", 1, "bc")]
    [InlineData("abc", 2, "c")]
    [InlineData("abc", 3, "")]
    [InlineData("", 0, "")]
    public void Slice_without_endIndex_Ok(string str, int startIndex, string expected)
    {
        Assert.Equal(expected, Util.Slice(str, startIndex));
    }

    [Theory]
    [InlineData("abc", 0, 0, "")]
    [InlineData("abc", 1, 2, "b")]
    [InlineData("abc", 2, 3, "c")]
    [InlineData("abc", 0, 5, "abc")]
    [InlineData("abc", 4, 2, "")]
    [InlineData("", 0, 2, "")]
    public void Slice_with_endIndex_Ok(string str, int startIndex, int endIndex, string expected)
    {
        Assert.Equal(expected, Util.Slice(str, startIndex, endIndex));
    }
}

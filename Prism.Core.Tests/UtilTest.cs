using System.Text.RegularExpressions;
using Xunit;

namespace Prism.Core.Tests;

public class UtilTest
{
    [Fact]
    public void Encode_StringToken_Ok()
    {
        var token = new StringToken("// <summary> it's comment </summary>", "comment", new []{"comment"});
        var actual = Util.Encode(token);
        Assert.Equal(token.Type, actual.Type);
        Assert.Equal(token.Alias, actual.Alias);
        var actualToken = Assert.IsType<StringToken>(actual);
        Assert.Equal("// &lt;summary> it's comment &lt;/summary>", actualToken.Content);
    }
    
    [Fact]
    public void Encode_StreamToken_Ok()
    {
        var token = new StreamToken(new []
        {
            new StringToken( "test it", "foo", new []{"foo"}),
            new StringToken("a & b", "bar", new []{"bar"}),
        }, "test_stream_token");
        var actual = Util.Encode(token);
        Assert.Equal(token.Type, actual.Type);
        Assert.Equal(token.Alias, actual.Alias);
        var actualToken = Assert.IsType<StreamToken>(actual);
        Assert.Equal(token.Content.Length, actualToken.Content.Length);
        var contentToken1 = Assert.IsType<StringToken>(actualToken.Content[1]);
        Assert.Equal("a &amp; b", contentToken1.Content);
    }

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
    
    
}
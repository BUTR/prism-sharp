using PrismSharp.Core;
using Xunit;

namespace PrismSharp.Highlighting.HTML.Tests;

public class HtmlHighlighterTest
{
    [Fact]
    public void Highlight_markup_entity_ok()
    {
        var htmlHighlighter = new HtmlHighlighter();
        const string code = @"
&amp;
&thetasym;
&#65;
&#x41;
&#x26f5;";
        const string expected = @"
<span class=""token entity named-entity"">&amp;amp;</span>
<span class=""token entity named-entity"">&amp;thetasym;</span>
<span class=""token entity"">&amp;#65;</span>
<span class=""token entity"">&amp;#x41;</span>
<span class=""token entity"">&amp;#x26f5;</span>";

        var html = htmlHighlighter.Highlight(code, LanguageGrammars.Markup, "html");
        Assert.Equal(expected, html);
    }

    [Fact]
    public void Highlight_C_hello_world_ok()
    {
        var htmlHighlighter = new HtmlHighlighter();
        const string code = @"
#include <stdio.h>

int main()
{
    printf(""Hello world!\n"");
    return 0;
}";
        const string expected = @"
<span class=""token directive-hash"">#</span><span class=""token directive keyword"">include</span> <span class=""token string"">&lt;stdio.h></span>

<span class=""token keyword"">int</span> <span class=""token function"">main</span><span class=""token punctuation"">(</span><span class=""token punctuation"">)</span>
<span class=""token punctuation"">{</span>
    <span class=""token function"">printf</span><span class=""token punctuation"">(</span><span class=""token string"">""Hello world!\n""</span><span class=""token punctuation"">)</span><span class=""token punctuation"">;</span>
    <span class=""token keyword"">return</span> <span class=""token number"">0</span><span class=""token punctuation"">;</span>
<span class=""token punctuation"">}</span>";

        var html = htmlHighlighter.Highlight(code, LanguageGrammars.C, "c");
        Assert.Equal(expected, html);
    }
}

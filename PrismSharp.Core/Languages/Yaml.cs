using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-yaml.js

public class Yaml : IGrammarDefinition
{
    // https://yaml.org/spec/1.2/spec.html#c-ns-anchor-property
    // https://yaml.org/spec/1.2/spec.html#c-ns-alias-node
    private const string AnchorOrAliasPattern = @"[*&][^\s[\]{},]+";
    // https://yaml.org/spec/1.2/spec.html#c-ns-tag-property
    private const string TagPattern = @"!(?:<[\w\-%#;/?:@&=+$,.!~*'()[\]]+>|(?:[a-zA-Z\d-]*!)?[\w\-%#;/?:@&=+$.~*'()]+)?";
    // https://yaml.org/spec/1.2/spec.html#c-ns-properties(n,c)
    private const string PropertiesPattern = @"(?:" + TagPattern + @"(?:[ \t]+" + AnchorOrAliasPattern + @")?|" +
                                             AnchorOrAliasPattern + @"(?:[ \t]+" + TagPattern + @")?)";

    private const string StrPattern = @"""(?:[^""\\\r\n]|\\.)*""|'(?:[^'\\\r\n]|\\.)*'";

    public Grammar Define()
    {
        // https://yaml.org/spec/1.2/spec.html#ns-plain(n,c)
        // This is a simplified version that doesn't support "#" and multiline keys
        // All these long scarry character classes are simplified versions of YAML's characters
        var plainKeyPattern =
            @"(?:[^\s\x00-\x08\x0e-\x1f!""#%&'*,\-:>?@[\]`{|}\x7f-\x84\x86-\x9f\ud800-\udfff\ufffe\uffff]|[?:-]<PLAIN>)(?:[ \t]*(?:(?![#:])<PLAIN>|:<PLAIN>))*"
                .Replace("<PLAIN>", @"[^\s\x00-\x08\x0e-\x1f,[\]{}\x7f-\x84\x86-\x9f\ud800-\udfff\ufffe\uffff]");

        return new Grammar
        {
            ["scalar"] = new GrammarToken[]
            {
                new(@"([\-:]\s*(?:\s<<prop>>[ \t]+)?[|>])[ \t]*(?:((?:\r?\n|\r)[ \t]+)\S[^\r\n]*(?:\2[^\r\n]+)*)"
                        .Replace("<<prop>>", PropertiesPattern),
                    true,
                    alias: new[] { "string" })
            },
            ["comment"] = new GrammarToken[] { new(@"#.*") },
            ["key"] = new GrammarToken[]
            {
                new(@"((?:^|[:\-,[{\r\n?])[ \t]*(?:<<prop>>[ \t]+)?)<<key>>(?=\s*:\s)"
                        .Replace("<<prop>>", PropertiesPattern)
                        .Replace("<<key>>", $"(?:{plainKeyPattern}|{StrPattern})"),
                    true,
                    true,
                    new[] { "atrule" })
            },
            ["directive"] = new GrammarToken[]
            {
                new(new Regex(@"(^[ \t]*)%.+", RegexOptions.Compiled | RegexOptions.Multiline),
                    true, alias: new[] { "important" })
            },
            ["datetime"] = new GrammarToken[]
            {
                new(
                    CreateValuePattern(
                        @"\d{4}-\d\d?-\d\d?(?:[tT]|[ \t]+)\d\d?:\d{2}:\d{2}(?:\.\d*)?(?:[ \t]*(?:Z|[-+]\d\d?(?::\d{2})?))?|\d{4}-\d{2}-\d{2}|\d\d?:\d{2}(?::\d{2}(?:\.\d*)?)?"),
                    true, alias: new[] { "number" })
            },
            ["boolean"] = new GrammarToken[]
            {
                new(CreateValuePattern(@"false|true", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    true, alias: new[] { "important" })
            },
            ["null"] = new GrammarToken[]
            {
                new(CreateValuePattern(@"null|~", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    true, alias: new[] { "important" })
            },
            ["string"] = new GrammarToken[]
            {
                new(CreateValuePattern(StrPattern), true, true)
            },
            ["number"] = new GrammarToken[]
            {
                new(
                    CreateValuePattern(
                        @"[+-]?(?:0x[\da-f]+|0o[0-7]+|(?:\d+(?:\.\d*)?|\.\d+)(?:e[+-]?\d+)?|\.inf|\.nan)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    true)
            },
            ["tag"] = new GrammarToken[] { new(TagPattern) },
            ["important"] = new GrammarToken[] { new(AnchorOrAliasPattern) },
            ["punctuation"] = new GrammarToken[] { new(@"---|[:[\]{}\-,|>?]|\.\.\.") },
        };
    }

    private static Regex CreateValuePattern(string value, RegexOptions flags = RegexOptions.Compiled)
    {
        var pattern = @"([:\-,[{]\s*(?:\s<<prop>>[ \t]+)?)(?:<<value>>)(?=[ \t]*(?:\r?$|,|\]|\}|(?:[\r\n]\s*)?#))"
            .Replace("<<prop>>", PropertiesPattern)
            .Replace("<<value>>", value);
        return new Regex(pattern, flags | RegexOptions.Multiline);
    }
}

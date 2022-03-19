using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PrismSharp.Core.Languages;
using Xunit;

namespace PrismSharp.Core.Tests;

public static class TestHelper
{
    public static async Task RunTestCaseFromFileAsync(Grammar testGrammar, string filePath)
    {
        var text = await File.ReadAllTextAsync(filePath);
        var testCase = ParseTestCaseFile(text);

        if (string.IsNullOrEmpty(testCase.Expected))
            throw new ArgumentException("The test case doesn't have an expected value");

        var expectedObj = JsonSerializer.Deserialize<JsonElement>(testCase.Expected);
        var expected = ToTokens(expectedObj);

        RunTestCase(testGrammar, testCase.Code, expected);
    }

    private static Token[] ToTokens(JsonElement rootEl)
    {
        if (rootEl.ValueKind != JsonValueKind.Array)
            throw new ArgumentException("Root element must be an array!", nameof(rootEl));

        var outList = new List<Token>();

        foreach (var el in rootEl.EnumerateArray())
        {
            if (el.ValueKind == JsonValueKind.String)
            {
                outList.Add(new StringToken(el.GetString()!));
                continue;
            }

            if (el.ValueKind != JsonValueKind.Array || el.GetArrayLength() != 2)
                continue;

            var type = el[0].GetString()!;
            var valEl = el[1];
            if (valEl.ValueKind == JsonValueKind.String)
            {
                outList.Add(new StringToken(valEl.GetString()!, type));
            }
            else
            {
                outList.Add(new StreamToken(ToTokens(valEl), type));
            }
        }

        return outList.ToArray();
    }

    private static TestCaseFile ParseTestCaseFile(string content)
    {
        var crlfRegex = new Regex(@"\r\n|\n", RegexOptions.Compiled);
        var eolMatch = crlfRegex.Match(content);
        var eol =  eolMatch.Success ? eolMatch.Groups[0].Value : "\n";

        // normalize line ends to CRLF
        content = new Regex(@"\r\n?|\n", RegexOptions.Compiled).Replace(content, "\r\n");

        var parts = new Regex(@"^-{10,}[ \t]*\r?$", RegexOptions.Compiled | RegexOptions.Multiline)
            .Split(content, 3);
        var code = parts.Length >= 1 ? parts[0] : string.Empty;
        var expected = parts.Length >= 2 ? parts[1] : string.Empty;
        var description = parts.Length == 3 ?  parts[2] : string.Empty;

        var file = new TestCaseFile(code.Trim(), expected.Trim(), description.Trim())
        {
            EOL = eol
        };

        var spaceRegex = new Regex(@"^\s*", RegexOptions.Compiled);
        var codeStartSpaces = spaceRegex.Match(code).Groups[0].Value;
        var expectedStartSpaces = spaceRegex.Match(expected).Groups[0].Value;
        var descriptionStartSpaces = spaceRegex.Match(description).Groups[0].Value;

        var rnRegex = new Regex(@"\r\n", RegexOptions.Compiled);
        var codeLineCount = rnRegex.Split(code).Length;
        var expectedLineCount = rnRegex.Split(expected).Length;

        file.CodeLineStart = rnRegex.Split(codeStartSpaces).Length;
        file.ExpectedLineStart = codeLineCount + rnRegex.Split(expectedStartSpaces).Length;
        file.DescriptionLineStart = codeLineCount + expectedLineCount + rnRegex.Split(descriptionStartSpaces).Length;

        return file;
    }


    public static void RunTestCase(Grammar testGrammar, string code, IReadOnlyList<Token> expected)
    {
        var tokens = Prism.Tokenize(code, testGrammar);
        var simpleTokens = Simplify(tokens);
        AssertDeepStrictEqual(simpleTokens, expected);
    }

    private static void AssertDeepStrictEqual(IReadOnlyList<Token> simpleTokens, IReadOnlyList<Token> expected)
    {
        Assert.NotNull(simpleTokens);
        Assert.Equal(expected.Count, simpleTokens.Count);

        for (var i = 0; i < expected.Count; i++)
        {
            var token = simpleTokens[i];
            var expectedToken = expected[i];

            if (expectedToken is StringToken expectedStringToken)
            {
                var stringToken = Assert.IsType<StringToken>(token);
                Assert.Equal(expectedStringToken.Type, stringToken.Type);
                Assert.Equal(expectedStringToken.Content, stringToken.Content);
                continue;
            }

            if (expectedToken is not StreamToken expectedStreamToken)
                continue;

            var streamToken = Assert.IsType<StreamToken>(token);
            AssertDeepStrictEqual(streamToken.Content, expectedStreamToken.Content);
        }
    }

    private static Token[] Simplify(IReadOnlyCollection<Token> tokens)
    {
        return tokens
            .Where(t => !IsBlankStringToken(t))
            .Select(InnerSimplify)
            .ToArray();
    }

    private static Token InnerSimplify(Token token)
    {
        return token switch
        {
            StringToken stringToken => new StringToken(stringToken.Content, stringToken.Type),
            StreamToken streamToken => new StreamToken(Simplify(streamToken.Content), streamToken.Type),
            _ => throw new ArgumentException("Type is not support!", nameof(token))
        };
    }

    private static bool IsBlankStringToken(Token token)
    {
        return token is StringToken stringToken && string.IsNullOrWhiteSpace(stringToken.Content);
    }
}

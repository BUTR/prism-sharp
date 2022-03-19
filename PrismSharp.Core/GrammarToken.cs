using System.Text.RegularExpressions;

namespace PrismSharp.Core;

/// <summary>
/// The expansion of a simple `RegExp` literal to support additional properties.
/// </summary>
public class GrammarToken
{
    /// <summary>
    /// The regular expression of the token.
    /// </summary>
    public Regex Pattern { get; set; }
    public bool Lookbehind { get; private set; }
    public bool Greedy { get; private set; }
    public string[] Alias { get; private set; }
    public Grammar? Inside { get; set; }


    public GrammarToken(string pattern,
        bool lookbehind = false,
        bool greedy = false,
        string[]? alias = null,
        Grammar? inside = null) : this(new Regex(pattern, RegexOptions.Compiled), lookbehind, greedy, alias, inside)
    {
    }

    public GrammarToken(Regex pattern,
        bool lookbehind = false,
        bool greedy = false,
        string[]? alias = null,
        Grammar? inside = null)
    {
        Pattern = pattern;
        Lookbehind = lookbehind;
        Greedy = greedy;
        Alias = alias ?? Array.Empty<string>();
        Inside = inside;
    }

}

using System.Text.RegularExpressions;

namespace PrismSharp.Core;

public static class Util
{
    public static string Slice(string str, int startIndex, int? endIndex = null)
    {
        if (startIndex >= str.Length) return string.Empty;
        if (!endIndex.HasValue || endIndex > str.Length)
            endIndex = str.Length;
        var sliceLength = endIndex.Value - startIndex;
        return str.Substring(startIndex, sliceLength);
    }

    public static MyMatch MatchPattern(Regex pattern, int pos, string text, bool lookbehind)
    {
        var match = pattern.Match(text, pos);
        var myMatch = new MyMatch(match);
        var matchGroups = myMatch.Groups;

        if (myMatch.Success && lookbehind && matchGroups.Length > 1 && matchGroups[1].Length > 0)
        {
            // change the match to remove the text matched by the Prism lookbehind group
            var lookbehindLength = matchGroups[1].Length;
            myMatch.Index += lookbehindLength;
            matchGroups[0] = Slice(matchGroups[0], lookbehindLength);
        }

        return myMatch;
    }

    public class MyMatch
    {
        public string[] Groups { get; set; }
        public int Index { get; set; }
        public bool Success { get; set; }

        public MyMatch(Match match)
        {
            Success = match.Success;
            Index = match.Index;
            Groups = ParseGroups(match);
        }

        // trim end with `\r`
        private static string[] ParseGroups(Match match) => match.Groups.OfType<Group>().Select(g => g.Value.TrimEnd('\r')).ToArray();
    }
}

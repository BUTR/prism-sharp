using System.Text.RegularExpressions;

namespace PrismSharp.Core;

public static class Util
{
    public static string Slice(string str, int startIndex, int? endIndex = null)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        if (startIndex >= str.Length) return string.Empty;
        return endIndex.HasValue
            ? str.Substring(startIndex, endIndex.Value - startIndex)
            : str.Substring(startIndex);
    }

    public static MyMatch MatchPattern(Regex pattern, int pos, string text, bool lookbehind)
    {
        var match = pattern.Match(text, pos);
        var myMatch = new MyMatch(match);
        var matchGroups = myMatch.Groups;

        if (myMatch.Success && lookbehind && matchGroups.Length > 1 &&
            !string.IsNullOrEmpty(matchGroups[1]))
        {
            // change the match to remove the text matched by the Prism lookbehind group
            var lookbehindLength = matchGroups[1].Length;
            myMatch.Index += lookbehindLength;
            matchGroups[0] = matchGroups[0].Substring(lookbehindLength);
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

        private string[] ParseGroups(Match match)
        {
            var list = new List<string>(match.Groups.Count);

            foreach (Group g in match.Groups)
            {
                list.Add(g.Value);
            }

            return list.ToArray();
        }
    }
}

using System.Text.RegularExpressions;

namespace PrismSharp.Core.Languages;

// From https://github.com/PrismJS/prism/blob/master/components/prism-lua.js

public class Lua : IGrammarDefinition
{
    public Grammar Define()
    {
      var grammar = new Grammar();

      grammar["comment"] = new GrammarToken[]
      {

        new(new Regex(@"^#!.+|--(?:\[(=*)\[[\s\S]*?\]\1\]|.*)", RegexOptions.Compiled | RegexOptions.Multiline), lookbehind: false, greedy: false),
      };
      grammar["string"] = new GrammarToken[]
      {

        new(@"([""'])(?:(?!\1)[^\\\r\n]|\\z(?:\r\n|\s)|\\(?:\r\n|[^z]))*\1|\[(=*)\[[\s\S]*?\]\2\]", lookbehind: false, greedy: true),
      };
      grammar["number"] = new GrammarToken[]
      {

        new(new Regex(@"\b0x[a-f\d]+(?:\.[a-f\d]*)?(?:p[+-]?\d+)?\b|\b\d+(?:\.\B|(?:\.\d*)?(?:e[+-]?\d+)?\b)|\B\.\d+(?:e[+-]?\d+)?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase), lookbehind: false, greedy: false),
      };
      grammar["keyword"] = new GrammarToken[]
      {

        new(@"\b(?:and|break|do|else|elseif|end|false|for|function|goto|if|in|local|nil|not|or|repeat|return|then|true|until|while)\b", lookbehind: false, greedy: false),
      };
      grammar["function"] = new GrammarToken[]
      {

        new(@"(?!\d)\w+(?=\s*(?:[({]))", lookbehind: false, greedy: false),
      };
      grammar["operator"] = new GrammarToken[]
      {

        new(@"[-+*%^&|#]|\/\/?|<[<=]?|>[>=]?|[=~]=?", lookbehind: false, greedy: false),
        new(@"(^|[^.])\.\.(?!\.)", lookbehind: true, greedy: false),
      };
      grammar["punctuation"] = new GrammarToken[]
      {

        new(@"[\[\](){},;]|\.+|:+", lookbehind: false, greedy: false),
      };

      return grammar;
    }
}

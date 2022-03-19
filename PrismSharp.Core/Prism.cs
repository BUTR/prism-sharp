namespace PrismSharp.Core;

public static class Prism
{
    public static Token[] Tokenize(string text, Grammar grammar)
    {
        var rest = grammar.Reset;
        if (rest is not null) {
            foreach (var kv in rest) {
                grammar[kv.Key] = kv.Value;
            }

            grammar.Reset = null;
        }

        var tokenList = new LinkedList<Token>();

        var head = new LinkedListNode<Token>(null!);
        var tail = new LinkedListNode<Token>(null!);

        tokenList.AddFirst(head);
        tokenList.AddLast(tail);
        AddAfter(tokenList, head, new StringToken(text));

        MatchGrammar(text, tokenList, grammar, head, 0);
        return tokenList.Where(t => t != null!).ToArray();
    }

    private static void MatchGrammar(string text, LinkedList<Token> tokenList, Grammar grammar,
        LinkedListNode<Token> startNode, int startPos, RematchOptions? rematch = null)
    {
        foreach (var kv in grammar)
        {
            var token = kv.Key;
            var patterns = kv.Value;

            for (var j = 0; j < patterns.Length; ++j)
            {
                if (rematch?.Cause == token + ',' + j)
                    return;

                var patternObj = patterns[j];
                var inside = patternObj.Inside;
                var lookbehind = patternObj.Lookbehind;
                var greedy = patternObj.Greedy;
                var alias = patternObj.Alias;

                var pattern = patternObj.Pattern;

                // iterate the token list and keep track of the current token/string position
                var currentNode = startNode.Next!;

                for (var pos = startPos;
                     currentNode != tokenList.Last;
                     pos += currentNode.Value.GetLength(), currentNode = currentNode.Next!)
                {
                    if (pos >= rematch?.Reach)
                        break;

                    var tokenListCount = tokenList.Count - 2; // excludes head, tail
                    if (tokenListCount > text.Length)
                        // Something went terribly wrong, ABORT, ABORT!
                        return;

                    var tokenVal = currentNode.Value;

                    // `tokenVal.Length > 0` is matched
                    if (tokenVal.IsMatchedToken() || tokenVal is not StringToken stringToken)
                        continue;

                    var str = stringToken.Content;

                    var removeCount = 1; // this is the to parameter of removeBetween
                    Util.MyMatch match;

                    if (greedy)
                    {
                        match = Util.MatchPattern(pattern, pos, text, lookbehind);
                        if (!match.Success || match.Index >= text.Length)
                            break;

                        var fromIdx = match.Index;
                        var to = match.Index + match.Groups[0].Length;
                        var p = pos;

                        // find the node that contains the match
                        p += currentNode.Value.GetLength();
                        while (fromIdx >= p)
                        {
                            currentNode = currentNode.Next!;
                            p += currentNode.Value.GetLength();
                        }

                        // adjust pos (and p)
                        p -= currentNode.Value.GetLength();
                        pos = p;

                        // the current node is a Token, then the match starts inside another Token, which is invalid
                        if (currentNode.Value.IsMatchedToken())
                            continue;

                        // find the last node which is affected by this match
                        for (
                            var k = currentNode;
                            k != tokenList.Last && (p < to || (!k.Value.IsMatchedToken() && k.Value is StringToken));
                            k = k.Next!)
                        {
                            removeCount++;
                            p += k.Value.GetLength();
                        }

                        removeCount--;

                        // replace with the new match
                        str = Util.Slice(text, pos, p);
                        match.Index -= pos;
                    }
                    else
                    {
                        match = Util.MatchPattern(pattern, 0, str, lookbehind);
                        if (!match.Success)
                            continue;
                    }

                    var from = match.Index;
                    var matchStr = match.Groups[0];
                    var before = Util.Slice(str, 0, from);
                    var after = Util.Slice(str, from + matchStr.Length);

                    var reach = pos + str.Length;
                    if (reach > rematch?.Reach)
                        rematch.Reach = reach;

                    var removeFrom = currentNode.Previous!;

                    if (before.Length > 0)
                    {
                        removeFrom = AddAfter(tokenList, removeFrom, new StringToken(before));
                        pos += before.Length;
                    }

                    RemoveRange(tokenList, removeFrom, removeCount);

                    Token wrapped = inside != null
                        ? new StreamToken(Tokenize(matchStr, inside), token, alias, matchStr)
                        : new StringToken(matchStr, token, alias, matchStr);

                    currentNode = AddAfter(tokenList, removeFrom, wrapped);

                    if (after.Length > 0)
                        AddAfter(tokenList, currentNode, new StringToken(after));

                    if (removeCount <= 1)
                        continue;

                    // at least one Token object was removed, so we have to do some rematching
                    // this can only happen if the current pattern is greedy

                    var nestedRematch = new RematchOptions
                    {
                        Cause = token + ',' + j,
                        Reach = reach
                    };
                    MatchGrammar(text, tokenList, grammar, currentNode.Previous!, pos, nestedRematch);

                    // the reach might have been extended because of the rematching
                    if (nestedRematch.Reach > rematch?.Reach)
                        rematch.Reach = nestedRematch.Reach;

                }
            }


        }
    }

    /// <summary>
    /// Adds a new node with the given value to the list.
    /// </summary>
    /// <param name="tokenList"></param>
    /// <param name="node"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    private static LinkedListNode<Token> AddAfter(LinkedList<Token> tokenList, LinkedListNode<Token> node, Token val)
    {
        var newNode = new LinkedListNode<Token>(val);
        tokenList.AddAfter(node, newNode);
        return newNode;
    }

    /// <summary>
    /// Removes `count` nodes after the given node. The given node will not be removed.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="node"></param>
    /// <param name="count"></param>
    private static void RemoveRange(LinkedList<Token> list, LinkedListNode<Token> node, int count)
    {
        var next = node.Next;
        for (var i = 0; i < count && next != list.Last && next is not null; i++)
        {
            list.Remove(next);
            next = node.Next;
        }
    }

    private class RematchOptions
    {
        public string Cause { get; set; } = null!;
        public int Reach { get; set; }
    }
}

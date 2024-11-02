using System.Collections;

namespace PrismSharp.Core;

public class Grammar : IEnumerable<KeyValuePair<string, GrammarToken[]>>
{
    private int _count;

    private IDictionary<string, OrderVal> GrammarTokenMap { get; }

    /// <summary>
    /// An optional grammar object that will be appended to this grammar.
    /// </summary>
    public Grammar? Reset { get; set; }

    public Grammar()
    {
        _count = 0;
        GrammarTokenMap = new Dictionary<string, OrderVal>(16);
    }

    public GrammarToken[] this[string key]
    {
        set
        {
            var wrapVal = GrammarTokenMap.TryGetValue(key, out var oldVal)
                ? new OrderVal(value, oldVal.PrevOrder, oldVal.Order)
                : new OrderVal(value, _count, ++_count);
            GrammarTokenMap[key] = wrapVal;
        }
    }

    private class OrderVal
    {
        public GrammarToken[] Val { get; }
        public float Order { get; }
        public float PrevOrder { get; set; }

        public OrderVal(GrammarToken[] val, float prevOrder, float order)
        {
            Val = val;
            PrevOrder = prevOrder;
            Order = order;
        }
    }

    public IEnumerator<KeyValuePair<string, GrammarToken[]>> GetEnumerator() => GrammarTokenMap
        .OrderBy(kv => kv.Value.Order)
        .Select(kv => new KeyValuePair<string, GrammarToken[]>(kv.Key, kv.Value.Val))
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

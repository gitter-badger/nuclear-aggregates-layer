using System.Collections;
using System.Collections.Generic;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class ConsistencyRuleCollection<TKey> : IEnumerable<IConsistencyRule>
    {
        private readonly List<IConsistencyRule> _items;
        private readonly TKey _key;

        public ConsistencyRuleCollection(TKey key)
        {
            _key = key;
            _items = new List<IConsistencyRule>();
        }

        public TKey Key 
        {
            get { return _key; }
        }

        public IEnumerator<IConsistencyRule> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public void Add(IConsistencyRule item)
        {
            _items.Add(item);
        }
    }
}
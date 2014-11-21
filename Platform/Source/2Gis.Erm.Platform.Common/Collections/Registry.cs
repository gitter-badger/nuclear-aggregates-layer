using System.Collections.Generic;
using System.Threading;

namespace DoubleGis.Erm.Platform.Common.Collections
{
    public abstract class Registry<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private SpinLock _lock = new SpinLock();

        public TValue GetOrAdd(TKey key)
        {
            TValue value;
            if (!_dictionary.TryGetValue(key, out value))
            {
                bool entered = false;
                try
                {
                    _lock.Enter(ref entered);

                    var snapshot = new Dictionary<TKey, TValue>(_dictionary);
                    if (!snapshot.TryGetValue(key, out value))
                    {
                        value = CreateValue(key);
                        snapshot.Add(key, value);
                        Interlocked.Exchange(ref _dictionary, snapshot);
                    }
                }
                finally
                {
                    if (entered)
                    {
                        _lock.Exit();
                    }
                }
            }

            return value;
        }

        protected abstract TValue CreateValue(TKey key);
    }
}

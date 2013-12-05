using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        protected readonly Func<T, TKey> KeyExtractor;

        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            this.KeyExtractor = keyExtractor;
        }

        public virtual bool Equals(T x, T y)
        {
            return this.KeyExtractor(x).Equals(this.KeyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return this.KeyExtractor(obj).GetHashCode();
        }
    }

    public class StrictKeyEqualityComparer<T, TKey> : KeyEqualityComparer<T, TKey>
        where TKey : IEquatable<TKey>
    {
        public StrictKeyEqualityComparer(Func<T, TKey> keyExtractor)
            : base(keyExtractor)
        {
        }

        public override bool Equals(T x, T y)
        {
            // This will use the overload that accepts a TKey parameter
            // instead of an object parameter.
            return this.KeyExtractor(x).Equals(this.KeyExtractor(y));
        }
    }
}

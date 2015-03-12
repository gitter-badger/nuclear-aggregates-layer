using System;
using System.Collections;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Common
{
    internal sealed class DelegateEnumerable<THit> : IEnumerable<THit>
    {
        private readonly Func<IEnumerator<THit>> _func;

        public DelegateEnumerable(Func<IEnumerator<THit>> func)
        {
            _func = func;
        }

        public IEnumerator<THit> GetEnumerator()
        {
            return _func();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
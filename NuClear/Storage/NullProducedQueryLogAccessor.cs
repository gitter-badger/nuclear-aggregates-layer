using System;

namespace NuClear.Storage
{
    public class NullProducedQueryLogAccessor : IProducedQueryLogAccessor
    {
        public Action<string> Log
        {
            get { return query => { }; }
        }
    }
}
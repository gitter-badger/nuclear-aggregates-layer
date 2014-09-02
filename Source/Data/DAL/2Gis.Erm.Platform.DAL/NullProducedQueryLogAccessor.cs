using System;

namespace DoubleGis.Erm.Platform.DAL
{
    public class NullProducedQueryLogAccessor : IProducedQueryLogAccessor
    {
        public Action<string> Log
        {
            get { return query => { }; }
        }
    }
}
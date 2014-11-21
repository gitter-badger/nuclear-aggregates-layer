using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure
{
    // FIXME {all, 23.10.2013}: выпилить, убрав также references на interception
    [Obsolete("если в бизнесс требованиях нужно логирование, то нужно его делать явно, также есть межанизмы OperationLogging и ActionLogging")]
    public class PropertyBag : IPropertyBag
    {
        private readonly Lazy<Dictionary<string, object>> _properties = new Lazy<Dictionary<string, object>>();

        public bool ContainsKey(string key)
        {
            return _properties.Value.ContainsKey(key);
        }

        public object GetProperty(string key)
        {
            return ContainsKey(key) ? _properties.Value[key] : null;
        }

        public void SetProperty(string key, object value)
        {
            _properties.Value[key] = value;
        }

        public bool RemovePropery(string key)
        {
            return _properties.Value.Remove(key);
        }
    }
}
using System;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure
{
    // FIXME {all, 23.10.2013}: выпилить, убрав также references на interception
    [Obsolete("если в бизнесс требованиях нужно логирование, то нужно его делать явно, также есть межанизмы OperationLogging и ActionLogging")]
    public interface IPropertyBag
    {
        bool ContainsKey(string key);
        object GetProperty(string key);
        void SetProperty(string key, object value);
        bool RemovePropery(string key);
    }
}
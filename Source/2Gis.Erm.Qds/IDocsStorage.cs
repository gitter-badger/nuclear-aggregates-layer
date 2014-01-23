using System.Collections.Generic;

namespace DoubleGis.Erm.Qds
{
    public interface IDocsStorage
    {
        IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc;
        void Update(IEnumerable<IDoc> docs);
    }
}
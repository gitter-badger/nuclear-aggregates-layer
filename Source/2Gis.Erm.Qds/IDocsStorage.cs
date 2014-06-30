using System.Collections.Generic;

namespace DoubleGis.Erm.Qds
{
    // TODO {f.zaharov, 20.06.2014}: Стоит подумать о том чтобы разбить на два интерфеса, для чтения и записи
    public interface IDocsStorage
    {
        IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc;
        TDoc GetById<TDoc>(string id) where TDoc : class, IDoc;

        void Update(IEnumerable<IDoc> docs);
        void Flush();
    }
}
using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl
{
    public interface IDocumentUpdater
    {
        void IndexDocuments(ICollection<long> id);
        void IndexAllDocuments();

        Type[] AffectedDocumentTypes { get; }
    }

    public interface IDocumentUpdaterFactory
    {
        IEnumerable<IDocumentUpdater> GetDocumentUpdatersForEntityType(Type entityType);
        IEnumerable<IDocumentUpdater> GetDocumentUpdatersForDocumentType(Type documentType);
    }
}
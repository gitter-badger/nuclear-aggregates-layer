using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IEntityToDocumentRelationFactory
    {
        IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForEntityType(Type entityType);
        IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForDocumentType(Type documentType);
    }
}
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public interface IRelationalDocsFinder
    {
        IEnumerable<TDoc> FindDocsByRelatedPart<TDoc>(IEntityKey part) where TDoc : class, IDoc;
        IEnumerable<TDoc> FindDocsByIndirectRelationPart<TDoc>(IEntityKey part) where TDoc : class, IDoc;
    }
}
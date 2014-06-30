using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocsUpdater
    {
        IEnumerable<IDoc> UpdateDocuments(IQueryable<IEntityKey> entities);
        Type SupportedDocType { get; }
    }
}
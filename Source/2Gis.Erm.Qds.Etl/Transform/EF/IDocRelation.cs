using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocRelation
    {
        Type DocType { get; }
        IEnumerable<Type> GetPartTypes();

        IDocsQuery GetByPartQuery(IEntityKey part);
    }
}
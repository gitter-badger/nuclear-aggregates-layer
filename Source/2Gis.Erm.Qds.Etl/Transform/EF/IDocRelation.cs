using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocRelation
    {
        Type GetDocType();
        Type[] GetPartTypes();
        IDocsQuery GetByPartQuery(IEntityKey part);
    }
}
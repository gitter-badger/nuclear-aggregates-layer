using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public interface IEntityLinkBuilder
    {
        IEnumerable<EntityLink> CreateEntityLinks(IChangeDescriptor changeDescriptor);
    }
}
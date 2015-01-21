using System.Collections.Generic;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IDependentEntityProvider
    {
        IEnumerable<IEntityType> GetDependentEntityNames(IEntityType entityName);
        IEnumerable<IEntityType> GetDependentEntities(IEntityType parentEntityName, IEntityType targetEntityName, long parentId);
        IEnumerable<IEntityType> GetDependentEntities(IEntityType parentEntityName, IEntityType targetEntityName, long parentId, bool forceCaching);
    }
}
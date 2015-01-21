using System.Collections.Generic;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IDependentEntityProvider
    {
        IEnumerable<IEntityType> GetDependentEntityNames(IEntityType entityName);
        IEnumerable<IEntityKey> GetDependentEntities(IEntityType parentEntityName, IEntityType targetEntityName, long parentId);
        IEnumerable<IEntityKey> GetDependentEntities(IEntityType parentEntityName, IEntityType targetEntityName, long parentId, bool forceCaching);
    }
}
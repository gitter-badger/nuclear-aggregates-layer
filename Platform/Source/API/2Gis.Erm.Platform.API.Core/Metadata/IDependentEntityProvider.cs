using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IDependentEntityProvider
    {
        IEnumerable<EntityName> GetDependentEntityNames(EntityName entityName);
        IEnumerable<IEntityKey> GetDependentEntities(EntityName parentEntityName, EntityName targetEntityName, long parentId);
        IEnumerable<IEntityKey> GetDependentEntities(EntityName parentEntityName, EntityName targetEntityName, long parentId, bool forceCaching);
    }
}
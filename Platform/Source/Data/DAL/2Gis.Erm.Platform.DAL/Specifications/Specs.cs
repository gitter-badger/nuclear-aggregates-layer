using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.DAL.Specifications
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<TEntity> ById<TEntity>(long id) where TEntity : class, IEntity, IEntityKey
            {
                return new FindSpecification<TEntity>(x => x.Id == id);
            }

            public static FindSpecification<TEntity> ById<TEntity>(long? id) where TEntity : class, IEntity, IEntityKey
            {
                return id.HasValue
                           ? new FindSpecification<TEntity>(x => x.Id == id.Value)
                           : new FindSpecification<TEntity>(x => false);
            }

            public static FindSpecification<TEntity> ByReplicationCode<TEntity>(Guid guid) where TEntity : class, IEntity, IReplicableEntity
            {
                return new FindSpecification<TEntity>(x => x.ReplicationCode == guid);
            }

            public static FindSpecification<TEntity> ByIds<TEntity>(IEnumerable<long> ids) where TEntity : class, IEntity, IEntityKey
            {
                return new FindSpecification<TEntity>(x => ids.Contains(x.Id));
            }

            public static FindSpecification<TEntity> ByReplicationCodes<TEntity>(IEnumerable<Guid> guids) where TEntity : class, IEntity, IReplicableEntity
            {
                return new FindSpecification<TEntity>(x => guids.Contains(x.ReplicationCode));
            }

            public static FindSpecification<TEntity> ActiveAndNotDeleted<TEntity>() where TEntity : class, IEntity, IDeletableEntity, IDeactivatableEntity
            {
                return new FindSpecification<TEntity>(x => x.IsActive && !x.IsDeleted);
            }

            public static FindSpecification<TEntity> NotDeleted<TEntity>() where TEntity : class, IEntity, IDeletableEntity
            {
                return new FindSpecification<TEntity>(x => !x.IsDeleted);
            }

            public static FindSpecification<TEntity> Active<TEntity>() where TEntity : class, IEntity, IDeactivatableEntity
            {
                return new FindSpecification<TEntity>(x => x.IsActive);
            }

            public static FindSpecification<TEntity> InactiveEntities<TEntity>() where TEntity : class, IEntity, IDeactivatableEntity
            {
                return new FindSpecification<TEntity>(x => !x.IsActive);
            }

            public static FindSpecification<TEntity> InactiveAndNotDeletedEntities<TEntity>() where TEntity : class, IEntity, IDeletableEntity, IDeactivatableEntity
            {
                return new FindSpecification<TEntity>(x => !x.IsActive && !x.IsDeleted);
            }

            public static FindSpecification<TEntity> InactiveOrDeletedEntities<TEntity>() where TEntity : class, IEntity, IDeletableEntity, IDeactivatableEntity
            {
                return new FindSpecification<TEntity>(x => !x.IsActive || x.IsDeleted);
            }

            public static FindSpecification<TEntity> Owned<TEntity>(long ownerCode) where TEntity : class, IEntity, ICuratedEntity
            {
                return new FindSpecification<TEntity>(x => x.OwnerCode == ownerCode);
            }

            public static FindSpecification<TEntity> Owned<TEntity>(IEnumerable<long> ownerCodes) where TEntity : class, IEntity, ICuratedEntity
            {
                return new FindSpecification<TEntity>(x => ownerCodes.Contains(x.OwnerCode));
            }

            public static FindSpecification<TEntity> NotOwned<TEntity>(long ownerCode) where TEntity : class, IEntity, ICuratedEntity
            {
                return new FindSpecification<TEntity>(x => x.OwnerCode != ownerCode);
            }

            public static FindSpecification<TEntity> NotOwned<TEntity>(IEnumerable<long> ownerCodes) where TEntity : class, IEntity, ICuratedEntity
            {
                return new FindSpecification<TEntity>(x => !ownerCodes.Contains(x.OwnerCode));
            }

            public static FindSpecification<TEntity> ByFileId<TEntity>(long fileId) where TEntity : class, IEntity, IEntityFile
            {
                return new FindSpecification<TEntity>(x => x.FileId == fileId);
            }

            public static FindSpecification<TEntity> ByFileIds<TEntity>(IEnumerable<long> fileIds) where TEntity : class, IEntity, IEntityFile
            {
                return new FindSpecification<TEntity>(x => fileIds.Contains(x.FileId));
            }

            public static FindSpecification<TEntity> ByOptionalFileId<TEntity>(long fileId) where TEntity : class, IEntity, IEntityFileOptional
            {
                return new FindSpecification<TEntity>(x => x.FileId == fileId);
            }

            public static FindSpecification<TEntity> Custom<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
            {
                return new FindSpecification<TEntity>(expression);
            }

            public static FindSpecification<TEntity> ExceptById<TEntity>(long id) where TEntity : class, IEntity, IEntityKey
            {
                return new FindSpecification<TEntity>(x => x.Id != id);
            }

            public static FindSpecification<TEntity> ExceptByIds<TEntity>(IEnumerable<long> ids) where TEntity : class, IEntity, IEntityKey
            {
                return new FindSpecification<TEntity>(x => !ids.Contains(x.Id));
            }
        }

        public static class Select
        {
            public static SelectSpecification<TEntity, long> Id<TEntity>() where TEntity : class, IEntity, IEntityKey
            {
                return new SelectSpecification<TEntity, long>(x => x.Id);
            }

            public static SelectSpecification<TEntity, long> Owner<TEntity>() where TEntity : class, IEntity, ICuratedEntity
            {
                return new SelectSpecification<TEntity, long>(x => x.OwnerCode);
            }
        }
    }
}
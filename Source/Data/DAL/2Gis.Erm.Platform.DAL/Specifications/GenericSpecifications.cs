using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.Specifications
{
    public static class GenericSpecifications
    {
        public static FindSpecification<TEntity> ById<TEntity>(long id) where TEntity : class, IEntity, IEntityKey
        {
            return new FindSpecification<TEntity>(x => x.Id == id);
        }

        public static FindSpecification<TEntity> ByIds<TEntity>(IEnumerable<long> ids) where TEntity : class, IEntity, IEntityKey
        {
            return new FindSpecification<TEntity>(x => ids.Contains(x.Id));
        }

        public static FindSpecification<TEntity> ActiveAndNotDeleted<TEntity>() where TEntity : class, IEntity, IDeletableEntity, IDeactivatableEntity
        {
            return new FindSpecification<TEntity>(x => x.IsActive && !x.IsDeleted);
        }

        public static FindSpecification<TEntity> InactiveEntities<TEntity>() where TEntity : class, IEntity, IDeletableEntity, IDeactivatableEntity
        {
            return new FindSpecification<TEntity>(x => !x.IsActive && !x.IsDeleted);
        }

        public static FindSpecification<TEntity> OwnedEntity<TEntity>(long ownerCode) where TEntity : class, IEntity, ICuratedEntity
        {
            return new FindSpecification<TEntity>(x => x.OwnerCode == ownerCode);
        }

        public static FindSpecification<TEntity> OwnedEntity<TEntity>(IEnumerable<long> ownerCodes) where TEntity : class, IEntity, ICuratedEntity
        {
            return new FindSpecification<TEntity>(x => ownerCodes.Contains(x.OwnerCode));
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

        public static SelectSpecification<TEntity, long> SelectId<TEntity>() where TEntity : class, IEntity, IEntityKey
        {
            return new SelectSpecification<TEntity, long>(x => x.Id);
        }

        public static SelectSpecification<TEntity, long> SelectOwner<TEntity>() where TEntity : class, IEntity, ICuratedEntity
        {
            return new SelectSpecification<TEntity, long>(x => x.OwnerCode);
        }
    }
}
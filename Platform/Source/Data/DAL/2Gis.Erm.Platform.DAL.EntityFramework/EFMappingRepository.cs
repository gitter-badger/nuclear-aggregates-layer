using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFMappingRepository<TEntity, TPersistentEntity> : EFRepository<TEntity, TPersistentEntity>, IRepository<TEntity>
        where TEntity : class, IEntity
        where TPersistentEntity : class, IEntity
    {
        private readonly IPersistenceChangesRegistryProvider _changesRegistryProvider;

        public EFMappingRepository(
            IUserContext userContext,
            IModifiableDomainContextProvider modifiableDomainContextProvider,
            IPersistenceChangesRegistryProvider changesRegistryProvider)
            : base(userContext, modifiableDomainContextProvider)
        {
            _changesRegistryProvider = changesRegistryProvider;
        }

        public void Add(TEntity entity)
        {
            AddRange(new[] { entity });
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            var clone = entities.ToList();

            Set().AddRange(clone.Select(entity =>
                {
                    ThrowIfEntityIsNull(entity, "entity");
                    ThrowIfEntityHasNoId(entity);

                    // NOTE: side effect
                    SetEntityAuditableInfo(entity, true);

                    return ConvertEntity(entity);
                })
                .ToList());

            var ids = clone.OfType<IEntityKey>().Select(x => x.Id).ToArray();
            if (ids.Length > 0)
            {
                _changesRegistryProvider.ChangesRegistry.Added<TEntity>(ids);
            }
        }

        public void Update(TEntity entity)
        {
            ThrowIfEntityIsNull(entity, "entity");

            // NOTE: side effect
            SetEntityAuditableInfo(entity, false);

            var persistentEntity = ConvertEntity(entity);

            EntityPlacementState state;
            var entry = EnsureEntityIsAttached(persistentEntity, out state);

            if (state == EntityPlacementState.CachedInContext)
            {
                entry.SetCurrentValues(persistentEntity);
            }
            else
            {
                entry.SetAsModified();
            }

            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Updated<TEntity>(entityKey.Id);
            }
        }

        public void Delete(TEntity entity)
        {
            DeleteRange(new[] { entity });
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            var clone = entities.ToList();

            Set().RemoveRange(clone.Select(entity =>
            {
                ThrowIfEntityIsNull(entity, "entity");
                ThrowIfEntityHasNoId(entity);

                // NOTE: side effect
                SetEntityAuditableInfo(entity, false);
                SetEntityDeleteableInfo(entity);

                var persistentEntity = ConvertEntity(entity);

                EntityPlacementState entityPlacementState;
                var entry = EnsureEntityIsAttached(persistentEntity, out entityPlacementState);

                if (entity is IDeletableEntity)
                {
                    if (entityPlacementState == EntityPlacementState.CachedInContext)
                    {
                        entry.SetCurrentValues(entity);
                    }
                    else
                    {
                        entry.SetAsModified();
                    }

                    return null;
                }

                return persistentEntity;
            }).Where(x => x != null).ToList());

            var ids = clone.OfType<IEntityKey>().Select(x => x.Id).ToArray();
            if (ids.Length > 0)
            {
                _changesRegistryProvider.ChangesRegistry.Deleted<TEntity>(ids);
            }
        }

        public int Save()
        {
            return SaveChanges();
        }

        private static TPersistentEntity ConvertEntity(TEntity entity)
        {
            // TODO: consider to inline via constructor
            return MappingRegistry.Map<TEntity, TPersistentEntity>(entity);
        }
    }
}
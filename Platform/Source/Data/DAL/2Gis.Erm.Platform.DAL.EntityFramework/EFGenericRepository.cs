using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFGenericRepository<TEntity> : EFRepository<TEntity, TEntity>, IRepository<TEntity>, IRepositoryProxy<TEntity>, IDomainEntityEntryAccessor<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IPersistenceChangesRegistryProvider _changesRegistryProvider;

        public EFGenericRepository(IUserContext userContext,
                                   IModifiableDomainContextProvider modifiableDomainContextProvider,
                                   IPersistenceChangesRegistryProvider changesRegistryProvider)
            : base(userContext, modifiableDomainContextProvider)
        {
            _changesRegistryProvider = changesRegistryProvider;
        }

        public void Add(TEntity entity)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)this;
            repositoryProxy.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)this;
            repositoryProxy.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)this;
            repositoryProxy.Update(entity, this);
        }

        public void Delete(TEntity entity)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)this;
            repositoryProxy.Delete(entity, this);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)this;
            repositoryProxy.DeleteRange(entities, this);
        }

        void IRepositoryProxy<TEntity>.Add(TEntity entity)
        {
            ThrowIfEntityIsNull(entity, "entity");
            ThrowIfEntityHasNoId(entity);
            
            SetEntityAuditableInfo(entity, true);

            Set().Add(entity);

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            // COMMENT {y.baranihin, 15.05.2014}: Зачем? Изменения в таких регистрируются как изменения родительского объекта.
            // COMMENT {a.rechkalov; y.baranihin, 21.05.2014}: Тут скорее важнее получить знание, что должно было быть изменено бизнес-логикой при выполнении операции, чтобы сравнить эти ожидания с реальными изменениями на уровне доступа к данным
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Added<TEntity>(entityKey.Id);
            }
        }

        void IRepositoryProxy<TEntity>.AddRange(IEnumerable<TEntity> entities)
        {
            var castedEntities = entities as TEntity[] ?? entities.ToArray();

            var entityIds = new List<long>();
            foreach (var entity in castedEntities)
            {
                ThrowIfEntityIsNull(entity, "entity");
                ThrowIfEntityHasNoId(entity);

                SetEntityAuditableInfo(entity, true);

                // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
                var entityKey = entity as IEntityKey;
                if (entityKey != null)
                {
                    entityIds.Add(entityKey.Id);
                }
            }

            Set().AddRange(castedEntities);

            entityIds.ForEach(x => _changesRegistryProvider.ChangesRegistry.Added<TEntity>(x));
        }

        void IRepositoryProxy<TEntity>.Update(TEntity entity, IDomainEntityEntryAccessor<TEntity> domainEntityEntryAccessor)
        {
            ThrowIfEntityIsNull(entity, "entity");

            EntityPlacementState entityPlacementState;
            var entry = domainEntityEntryAccessor.GetDomainEntityEntry(entity, out entityPlacementState);

            SetEntityAuditableInfo(entity, false);

            if (entityPlacementState == EntityPlacementState.CachedInContext)
            {
                entry.SetCurrentValues(entity);
            }
            else
            {
                entry.SetAsModified();
            }

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Updated<TEntity>(entityKey.Id);
            }
        }

        void IRepositoryProxy<TEntity>.Delete(TEntity entity, IDomainEntityEntryAccessor<TEntity> domainEntityEntryAccessor)
        {
            ThrowIfEntityIsNull(entity, "entity");

            EntityPlacementState entityPlacementState;
            var entry = domainEntityEntryAccessor.GetDomainEntityEntry(entity, out entityPlacementState);

            if (entity is IDeletableEntity)
            {
                SetEntityAuditableInfo(entity, false);
                SetEntityDeleteableInfo(entity);

                if (entityPlacementState == EntityPlacementState.CachedInContext)
                {
                    entry.SetCurrentValues(entity);
                }
                else
                {
                    entry.SetAsModified();
                }
            }
            else
            {
                // physically delete from database
                Set().Remove((TEntity)entry.Entity);
            }

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Deleted<TEntity>(entityKey.Id);
            }
        }

        void IRepositoryProxy<TEntity>.DeleteRange(IEnumerable<TEntity> entities, IDomainEntityEntryAccessor<TEntity> domainEntityEntryAccessor)
        {
            var castedEntities = entities as TEntity[] ?? entities.ToArray();

            var entitiesToDeletePhysically = new List<TEntity>();
            var entityIds = new List<long>();
            foreach (var entity in castedEntities)
            {
                EntityPlacementState entityPlacementState;
                var entry = domainEntityEntryAccessor.GetDomainEntityEntry(entity, out entityPlacementState);

                if (entity is IDeletableEntity)
                {
                    SetEntityAuditableInfo(entity, false);
                    SetEntityDeleteableInfo(entity);

                    if (entityPlacementState == EntityPlacementState.CachedInContext)
                    {
                        entry.SetCurrentValues(entity);
                    }
                    else
                    {
                        entry.SetAsModified();
                    }
                }
                else
                {
                    entitiesToDeletePhysically.Add((TEntity)entry.Entity);
                }

                // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
                var entityKey = entity as IEntityKey;
                if (entityKey != null)
                {
                    entityIds.Add(entityKey.Id);
                }
            }

            Set().RemoveRange(entitiesToDeletePhysically);

            entityIds.ForEach(x => _changesRegistryProvider.ChangesRegistry.Deleted<TEntity>(x));
        }

        IDbEntityEntry IDomainEntityEntryAccessor<TEntity>.GetDomainEntityEntry(TEntity entity, out EntityPlacementState entityPlacementState)
        {
            return EnsureEntityIsAttached(entity, out entityPlacementState);
                    }

        public int Save()
            {
            return SaveChanges();
        }
    }
}
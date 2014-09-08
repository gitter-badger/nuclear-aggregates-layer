using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFGenericRepository<TEntity> : EFRepository<TEntity>, IRepository<TEntity>, IRepositoryProxy<TEntity>, IDomainEntityEntryAccessor<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IUserContext _userContext;
        private readonly IDomainContextSaveStrategy _domainContextSaveStrategy;
        private readonly IPersistenceChangesRegistryProvider _changesRegistryProvider;

        public EFGenericRepository(IUserContext userContext,
                                   IModifiableDomainContextProvider modifiableDomainContextProvider,
                                   IDomainContextSaveStrategy domainContextSaveStrategy,
                                   IPersistenceChangesRegistryProvider changesRegistryProvider)
            : base(modifiableDomainContextProvider)
        {
            _userContext = userContext;
            _domainContextSaveStrategy = domainContextSaveStrategy;
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

        public int Save()
        {
            return _domainContextSaveStrategy.IsSaveDeferred ? 0 : DomainContext.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
        }

        void IRepositoryProxy<TEntity>.Add(TEntity entity)
        {
            CheckArgumentNull(entity, "entity");
            CheckArgumentIdentifier(entity);

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
                CheckArgumentNull(entity, "entity");
                CheckArgumentIdentifier(entity);

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
            CheckArgumentNull(entity, "entity");

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
            CheckArgumentNull(entity, "entity");

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
            var cachedEntity = DomainContext.Set<TEntity>().Local.SingleOrDefault(x => x.Equals(entity));
            if (cachedEntity != null)
            {
                var entry = DomainContext.Entry(cachedEntity);
                if (entry.State != EntityState.Unchanged)
                {
                    // т.е. объект помечен как измененный => applychanges для него не выполнялся
                    if (!_domainContextSaveStrategy.IsSaveDeferred)
                    {
                        var entityKey = entity as IEntityKey;

                        // используется НЕотложенное сохранение - т.е. объект изменили, не сохранили изменения и опять пытаемся менять экземпляр сущности с тем же identity
                        throw new InvalidOperationException(string.Format("Instance of type {0} with id={1} already in domain context cache " +
                                                                          "with unsaved changes => trying to update not saved entity. " +
                                                                          "Possible entity repository save method not called before next update. " +
                                                                          "Save mode is immediately, not deferred",
                                                                          typeof(TEntity).Name,
                                                                          entityKey != null ? entityKey.Id.ToString() : "NOTDETECTED"));
                    }

                    // сохрание отложенное - пытаемся обновить несохраненную сущность, это не хорошо, но пока разрешено, 
                    // т.к. возможно отложенный save для EF context ещё просто не вызывался
                }

                entityPlacementState = EntityPlacementState.CachedInContext;

                return new EFEntityEntry(entry);
            }
            else
            {
                var entry = DomainContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    Set().Attach(entity);
                }

                entityPlacementState = EntityPlacementState.AttachedToContext;
                return new EFEntityEntry(entry);
            }
        }

        private static void CheckArgumentIdentifier(TEntity entity)
        {
            var entityWithId = entity as IEntityKey;
            if (entityWithId != null && entityWithId.Id == 0)
            {
                throw new InvalidOperationException("Saving entity without pregenerated identity is not allowed");
            }
        }

        private static void SetEntityDeleteableInfo(TEntity entity)
        {
            // deactivate before deleting
            var deactivatableEntity = entity as IDeactivatableEntity;
            if (deactivatableEntity != null)
            {
                deactivatableEntity.IsActive = false;
            }

            var deletableEntity = entity as IDeletableEntity;
            if (deletableEntity == null)
            {
                return;
            }

            // logically delete from database
            deletableEntity.IsDeleted = true;
        }

        private void SetEntityAuditableInfo(TEntity entity, bool isEntityCreated)
        {
            var auditableEntity = entity as IAuditableEntity;
            if (auditableEntity == null)
            {
                return;
            }

            var now = DateTime.UtcNow;

            if (isEntityCreated)
            {
                auditableEntity.CreatedOn = now;
                auditableEntity.CreatedBy = _userContext.Identity.Code;
            }

            auditableEntity.ModifiedOn = now;
            auditableEntity.ModifiedBy = _userContext.Identity.Code;
        }

        private DbSet<TEntity> Set()
        {
            return DomainContext.Set<TEntity>();
        }
    }
}
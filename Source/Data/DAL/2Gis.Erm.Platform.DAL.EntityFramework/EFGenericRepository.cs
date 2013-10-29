using System;
using System.Data;
using System.Data.Objects;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFGenericRepository<TEntity> : EFRepository<TEntity>, IRepository<TEntity>, IRepositoryProxy<TEntity>, IDomainEntityAccessor<TEntity>
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

        public int Save()
        {
            return _domainContextSaveStrategy.IsSaveDeferred ? 0 : DomainContext.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
        }

        void IRepositoryProxy<TEntity>.Add(TEntity entity)
        {
            CheckArgumentNull(entity, "entity");
            CheckArgumentIdentifier(entity);
            SetEntityAuditableInfo(entity, true);
            GetObjectSet().AddObject(entity);
            DomainContext.ChangeObjectState(entity, EntityState.Added);
            _changesRegistryProvider.ChangesRegistry.Added<TEntity>(entity.GetId());
        }

        void IRepositoryProxy<TEntity>.Update(TEntity entity, IDomainEntityAccessor<TEntity> domainEntityAccessor)
        {
            CheckArgumentNull(entity, "entity");
            var domainEntity = domainEntityAccessor.GetDomainEntity(entity);
            SetEntityAuditableInfo(domainEntity, false);
            DomainContext.ChangeObjectState(domainEntity, EntityState.Modified);
            GetObjectSet().ApplyCurrentValues(domainEntity);
            _changesRegistryProvider.ChangesRegistry.Updated<TEntity>(entity.GetId());
        }

        void IRepositoryProxy<TEntity>.Delete(TEntity entity, IDomainEntityAccessor<TEntity> domainEntityAccessor)
        {
            CheckArgumentNull(entity, "entity");
            var domainEntity = domainEntityAccessor.GetDomainEntity(entity);

            var deletableEntity = entity as IDeletableEntity;
            if (deletableEntity == null)
            {
                // physically delete from database
                GetObjectSet().DeleteObject(domainEntity);
            }
            else
            {
                SetEntityAuditableInfo(domainEntity, false);

                // deactivate before deleting
                var deactivatableEntity = domainEntity as IDeactivatableEntity;
                if (deactivatableEntity != null)
                {
                    deactivatableEntity.IsActive = false;
                }

                // logically delete from database
                deletableEntity.IsDeleted = true;

                DomainContext.ChangeObjectState(domainEntity, EntityState.Modified);
                GetObjectSet().ApplyCurrentValues(domainEntity);
            }

            _changesRegistryProvider.ChangesRegistry.Deleted<TEntity>(entity.GetId());
        }

        TEntity IDomainEntityAccessor<TEntity>.GetDomainEntity(TEntity entity)
        {
            var entityKey = GetEntityKey(entity);
            var objectStateEntry = GetObjectStateEntry(entityKey);

            // Если состояние не null, значит уже есть копия объекта в кеше EF context'a
            if (objectStateEntry != null)
            {
                if (objectStateEntry.State != EntityState.Unchanged)
                {
                    // т.е. объект помечен как измененный => applychanges для него не выполнялся
                    if (!_domainContextSaveStrategy.IsSaveDeferred)
                    {
                        var entityAsEntityKey = entity as IEntityKey;

                        // используется НЕотложенное сохранение - т.е. объект изменили, не сохранили изменения и опять пытаемся менять экземпляр сущности с тем же identity
                        throw new InvalidOperationException(string.Format("Instance of type {0} with id={1} already in domain context cache " +
                                                                          "with unsaved changes => trying to update not saved entity. " +
                                                                          "Possible entity repository save method not called before next update. " +
                                                                          "Save mode is immediately, not deferred",
                                                                          typeof(TEntity).Name,
                                                                          entityAsEntityKey != null ? entityAsEntityKey.Id.ToString() : "NOTDETECTED"));
                    }

                    // сохрание отложенное - пытаемся обновить несохраненную сущность, это не хорошо, но пока разрешено, 
                    // т.к. возможно отложенный save для EF context ещё просто не вызывался
                }

                // чтобы не потерять новые изменения нужно сущность из EF контекста отцепить
                // Берем объект из кэша
                var contextCachedEntity = (TEntity)DomainContext.GetObjectByKey(entityKey);
                Detach(contextCachedEntity); // в новых версия EF можно так dbContext.Entry(entity).State = EntityState.Detached;
            }

            // делаем attach всегда, т.к. используется notracking при выборках
            // а повторные update возможно только после предварительного deattach
            // способ избежать этого - можно обновить значения экземпляра уже находящегося в cache EF context, например если использовать STE сущности или другие механизмы
            // пока мы считаем что используем POCO - поэтому присидания с Attach\Deattach
            Attach(entity);
            return entity;
        }

        private EFEntityStateEntry GetObjectStateEntry(EntityKey entityKey)
        {
            EFEntityStateEntry stateEntry;
            DomainContext.TryGetObjectStateEntry(entityKey, out stateEntry);
            return stateEntry;
        }

        private void Attach(TEntity entity)
        {
            var entityKey = GetEntityKey(entity);
            if (GetObjectStateEntry(entityKey) == null)
            {
                GetObjectSet().Attach(entity);
            }
        }

        private void Detach(TEntity entity)
        {
            GetObjectSet().Detach(entity);
        }

        private EntityKey GetEntityKey(TEntity entity)
        {
            var objectSet = GetObjectSet();
            var keyMembers = objectSet.EntitySet.ElementType.KeyMembers;
            var entityKeyMembers = (from keyMember in keyMembers
                                    let keyMemberValue = entity.GetType().GetProperty(keyMember.Name).GetValue(entity, null)
                                    select new EntityKeyMember(keyMember.Name, keyMemberValue))
                .ToArray();
            return new EntityKey(DomainContext.DefaultContextName + "." + objectSet.EntitySet.Name, entityKeyMembers);
        }

        private IObjectSet<TEntity> GetObjectSet()
        {
            var objectSet = DomainContext.CreateObjectSet<TEntity>();
            objectSet.MergeOption = MergeOption.NoTracking;
            return objectSet;
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

        private static void CheckArgumentIdentifier(TEntity entity)
        {
            var entityWithId = entity as IEntityKey;
            if (entityWithId != null && entityWithId.Id == 0)
            {
                throw new InvalidOperationException("Saving entity without pregenerated identity is not allowed");
            }
        }
    }
}
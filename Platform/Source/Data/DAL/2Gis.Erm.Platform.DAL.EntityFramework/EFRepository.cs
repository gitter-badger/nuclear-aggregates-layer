using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AutoMapper;

using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EFRepository<TDomainEntity, TPersistentEntity> : IRepository<TDomainEntity>
        where TDomainEntity : class, IEntity
        where TPersistentEntity : class, IEntity
    {
        private readonly IUserContext _userContext;
        private readonly IModifiableDomainContextProvider _domainContextProvider;
        private readonly IPersistenceChangesRegistryProvider _changesRegistryProvider;
        private readonly IMappingEngine _mappingEngine;

        private readonly Lazy<EFDomainContext> _domainContext;

        public EFRepository(IUserContext userContext, 
                               IModifiableDomainContextProvider domainContextProvider, 
                               IPersistenceChangesRegistryProvider changesRegistryProvider, 
                               IMappingEngine mappingEngine)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException("userContext");
            }

            if (domainContextProvider == null)
            {
                throw new ArgumentNullException("domainContextProvider");
            }

            _userContext = userContext;
            _domainContextProvider = domainContextProvider;
            _changesRegistryProvider = changesRegistryProvider;
            _mappingEngine = mappingEngine;
            _domainContext = new Lazy<EFDomainContext>(GetDomainContext);
        }

        public void Add(TDomainEntity entity)
        {
            ThrowIfEntityIsNull(entity, "entity");
            ThrowIfEntityHasNoId(entity);

            SetEntityReplicableInfo(entity);
            SetEntityAuditableInfo(entity, true);

            _domainContext.Value.Add(ConvertToPersistent(entity));

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            // COMMENT {y.baranihin, 15.05.2014}: Зачем? Изменения в таких регистрируются как изменения родительского объекта.
            // COMMENT {a.rechkalov; y.baranihin, 21.05.2014}: Тут скорее важнее получить знание, что должно было быть изменено бизнес-логикой при выполнении операции, чтобы сравнить эти ожидания с реальными изменениями на уровне доступа к данным
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                // FIXME {all, 27.01.2015}: Скорее всего правильнее будет регистрировать изменения persistent entity. Здесь и ниже оставлен старый вариант, чтобы не вносить лишнюю регрессию
                _changesRegistryProvider.ChangesRegistry.Added<TDomainEntity>(entityKey.Id);
            }
        }

        public void AddRange(IEnumerable<TDomainEntity> entities)
        {
            var persistentEntities = new List<TPersistentEntity>();
            var entityIds = new List<long>();
            foreach (var entity in entities)
            {
                ThrowIfEntityIsNull(entity, "entity");
                ThrowIfEntityHasNoId(entity);

                SetEntityReplicableInfo(entity);
                SetEntityAuditableInfo(entity, true);

                persistentEntities.Add(ConvertToPersistent(entity));

                // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
                var entityKey = entity as IEntityKey;
                if (entityKey != null)
                {
                    entityIds.Add(entityKey.Id);
                }
            }

            _domainContext.Value.AddRange(persistentEntities);

            if (entityIds.Count > 0)
            {
                _changesRegistryProvider.ChangesRegistry.Added<TDomainEntity>(entityIds.ToArray());
            }
        }

        public void Update(TDomainEntity entity)
        {
            ThrowIfEntityIsNull(entity, "entity");

            SetEntityAuditableInfo(entity, false);

            _domainContext.Value.Update(ConvertToPersistent(entity));

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Updated<TDomainEntity>(entityKey.Id);
            }
        }

        public void Delete(TDomainEntity entity)
        {
            ThrowIfEntityIsNull(entity, "entity");

            SetEntityAuditableInfo(entity, false);
            SetEntityDeleteableInfo(entity);
            var persistentEntity = ConvertToPersistent(entity);
            if (persistentEntity is IDeletableEntity)
            {
                _domainContext.Value.Update(persistentEntity);
            }
            else
            {
                _domainContext.Value.Remove(persistentEntity);
            }

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Deleted<TDomainEntity>(entityKey.Id);
        }
        }

        public void DeleteRange(IEnumerable<TDomainEntity> entities)
        {
            var entitiesToDeletePhysically = new List<TPersistentEntity>();
            var entityIds = new List<long>();
            foreach (var entity in entities)
            {
                SetEntityAuditableInfo(entity, false);
                SetEntityDeleteableInfo(entity);
                var persistentEntity = ConvertToPersistent(entity);
                if (persistentEntity is IDeletableEntity)
                {
                    _domainContext.Value.Update(persistentEntity);
                }
                else
                {
                    entitiesToDeletePhysically.Add(persistentEntity);
                }

                // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
                var entityKey = entity as IEntityKey;
                if (entityKey != null)
                {
                    entityIds.Add(entityKey.Id);
                }
                }

            _domainContext.Value.RemoveRange(entitiesToDeletePhysically);

            entityIds.ForEach(x => _changesRegistryProvider.ChangesRegistry.Deleted<TDomainEntity>(x));
        }

        public int Save()
        {
            return _domainContext.Value.SaveChanges();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowIfEntityIsNull(TDomainEntity value, string parameterName)
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowIfEntityHasNoId(TDomainEntity entity)
        {
            var entityWithId = entity as IEntityKey;
            if (entityWithId != null && entityWithId.Id == 0)
            {
                throw new InvalidOperationException("Saving entity without pregenerated identity is not allowed");
            }
        }

        private static void SetEntityDeleteableInfo(IEntity entity)
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

        private static void SetEntityReplicableInfo(IEntity entity)
            {
            var replicableEntity = entity as IReplicableEntity;
            if (replicableEntity == null || replicableEntity.ReplicationCode != Guid.Empty)
            {
                return;
            }

            replicableEntity.ReplicationCode = Guid.NewGuid();
        }

        private void SetEntityAuditableInfo(IEntity entity, bool isEntityCreated)
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

        private EFDomainContext GetDomainContext()
        {
            var context = _domainContextProvider.Get<TPersistentEntity>() as EFDomainContext;
            if (context == null)
            {
                throw new ApplicationException("IDbContext implementation must inherit from ObjectContext");
            }

            return context;
            }

        private TPersistentEntity ConvertToPersistent(TDomainEntity entity)
        {
            return typeof(TPersistentEntity) == typeof(TDomainEntity) ? (TPersistentEntity)(object)entity : _mappingEngine.Map<TDomainEntity, TPersistentEntity>(entity);
        }
    }
}

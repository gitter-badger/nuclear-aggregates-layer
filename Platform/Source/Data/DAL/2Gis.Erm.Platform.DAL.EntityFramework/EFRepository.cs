using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public abstract class EFRepository<TEntity, TPersistentEntity>
        where TEntity : class, IEntity
        where TPersistentEntity : class, IEntity
    {
        private readonly IUserContext _userContext;
        private readonly IModifiableDomainContextProvider _domainContextProvider;
        private EFDomainContext _context;

        protected EFRepository(IUserContext userContext,
                               IModifiableDomainContextProvider domainContextProvider)
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
        }

        private EFDomainContext DomainContext
        {
            get
            {
                if (_context != null)
                {
                    return _context;
                }

                _context = _domainContextProvider.Get<TPersistentEntity>() as EFDomainContext;
                if (_context == null)
                {
                    throw new ApplicationException("IDbContext implementation must inherit from ObjectContext");
                }

                return _context;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ThrowIfEntityIsNull(TEntity value, string parameterName)
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void ThrowIfEntityHasNoId(TEntity entity)
        {
            var entityWithId = entity as IEntityKey;
            if (entityWithId != null && entityWithId.Id == 0)
            {
                throw new InvalidOperationException("Saving entity without pregenerated identity is not allowed");
            }
        }

        protected int SaveChanges()
        {
            return ((IModifiableDomainContext)DomainContext).SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
        }

        protected DbSet<TPersistentEntity> Set()
        {
            return DomainContext.Set<TPersistentEntity>();
        }

        protected IDbEntityEntry EnsureEntityIsAttached(TPersistentEntity entity, out EntityPlacementState entityPlacementState)
        {
            var cachedEntity = DomainContext.Set<TPersistentEntity>().Local.SingleOrDefault(x => x.Equals(entity));
            if (cachedEntity != null)
            {
                var entry = DomainContext.Entry(cachedEntity);
                if (entry.State != EntityState.Unchanged)
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

                entityPlacementState = EntityPlacementState.CachedInContext;
                return new EFEntityEntry(entry);
            }
            else
            {
                var entry = DomainContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    DomainContext.Set<TPersistentEntity>().Attach(entity);
                }

                entityPlacementState = EntityPlacementState.AttachedToContext;
                return new EFEntityEntry(entry);
            }
        }

        protected void SetEntityAuditableInfo(TEntity entity, bool isEntityCreated)
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

        protected void SetEntityDeleteableInfo(TEntity entity)
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
    }
}

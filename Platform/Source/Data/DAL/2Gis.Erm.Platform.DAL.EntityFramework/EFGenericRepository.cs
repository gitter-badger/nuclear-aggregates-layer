using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFGenericRepository<TEntity> : EFRepository<TEntity, TEntity>, IRepository<TEntity>
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
            ThrowIfEntityIsNull(entity, "entity");
            ThrowIfEntityHasNoId(entity);

            SetEntityAuditableInfo(entity, true);

            DomainContext.Add(entity);

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            // COMMENT {y.baranihin, 15.05.2014}: Зачем? Изменения в таких регистрируются как изменения родительского объекта.
            // COMMENT {a.rechkalov; y.baranihin, 21.05.2014}: Тут скорее важнее получить знание, что должно было быть изменено бизнес-логикой при выполнении операции, чтобы сравнить эти ожидания с реальными изменениями на уровне доступа к данным
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Added<TEntity>(entityKey.Id);
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
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

            DomainContext.AddRange(castedEntities);

            entityIds.ForEach(x => _changesRegistryProvider.ChangesRegistry.Added<TEntity>(x));
        }

        public void Update(TEntity entity)
        {
            ThrowIfEntityIsNull(entity, "entity");

            SetEntityAuditableInfo(entity, false);

            DomainContext.Update(entity);

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Updated<TEntity>(entityKey.Id);
            }
        }

        public void Delete(TEntity entity)
        {
            ThrowIfEntityIsNull(entity, "entity");

            if (entity is IDeletableEntity)
            {
                SetEntityDeleteableInfo(entity);


                SetEntityAuditableInfo(entity, false);

                DomainContext.Update(entity);
            }
            else
            {
                DomainContext.Remove(entity);
            }

            // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
            var entityKey = entity as IEntityKey;
            if (entityKey != null)
            {
                _changesRegistryProvider.ChangesRegistry.Deleted<TEntity>(entityKey.Id);
            }
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            var castedEntities = entities as TEntity[] ?? entities.ToArray();

            var entitiesToDeletePhysically = new List<TEntity>();
            var entityIds = new List<long>();
            foreach (var entity in castedEntities)
            {
                if (entity is IDeletableEntity)
                {
                    SetEntityDeleteableInfo(entity);
                    SetEntityAuditableInfo(entity, false);
                    DomainContext.Update(entity);
                }
                else
                {
                    entitiesToDeletePhysically.Add(entity);
                }

                // TODO {all, 29.04.2014}: необходимо регистрировать изменения объектов без Id
                var entityKey = entity as IEntityKey;
                if (entityKey != null)
                {
                    entityIds.Add(entityKey.Id);
                }
            }

            DomainContext.RemoveRange(entitiesToDeletePhysically);

            entityIds.ForEach(x => _changesRegistryProvider.ChangesRegistry.Deleted<TEntity>(x));
        }

        public int Save()
        {
            return DomainContext.SaveChanges();
        }
    }
}
using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public interface IBatchDeletePersistenceService : ISimplifiedPersistenceService
    {
        void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity, IEntityKey;
        void Delete<TEntity>(IEnumerable<TEntity> entities, IReadOnlyList<EntityKeyExtractor<TEntity>> keyExtractors) where TEntity : class, IEntity;
    }

    public sealed class EntityKeyExtractor<TEntity> where TEntity : class, IEntity
    {
        public string KeyName { get; set; }
        public Func<TEntity, string> KeyValueExtractor { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Core.ActionLogging
{
    public sealed class ChangesDescriptor
    {
        private readonly IEntityType _entityType;
        private readonly long _entityId;
        private readonly IReadOnlyDictionary<string, PropertyChangeDescriptor> _changes;

        private ChangesDescriptor(IEntityType entityType, long entityId, IReadOnlyDictionary<string, PropertyChangeDescriptor> changes)
        {
            _entityType = entityType;
            _entityId = entityId;
            _changes = changes;
        }

        public IEntityType EntityType
        {
            get { return _entityType; }
        }

        public long EntityId
        {
            get { return _entityId; }
        }

        public IReadOnlyDictionary<string, PropertyChangeDescriptor> Changes
        {
            get { return _changes; }
        }
        
        public static ChangesDescriptor Create<TEntity>(TEntity originalObject, TEntity modifiedObject) where TEntity : class, IEntity
        {
            long? originalObjectId = originalObject != null ? (long?)originalObject.GetId() : null;
            long? modifiedObjectId = modifiedObject != null ? (long?)modifiedObject.GetId() : null;
            if (!originalObjectId.HasValue && !modifiedObjectId.HasValue)
            {
                throw new ArgumentException("Can't extract Id from original or modified object");
            }

            if (originalObjectId.HasValue && modifiedObjectId.HasValue && originalObjectId.Value != modifiedObjectId.Value)
            {
                throw new ArgumentException(string.Format("Can't id from original {0} and modified object {1} are not equal", originalObjectId.Value, modifiedObjectId.Value));
            }

            var entityId = originalObjectId.HasValue
                            ? originalObjectId.Value
                            : modifiedObjectId.Value;

            var entityType = typeof(TEntity).AsEntityName();
            var changes = CompareObjectsHelper.CompareObjects(CompareObjectMode.Shallow, originalObject, modifiedObject, Enumerable.Empty<string>());

            return new ChangesDescriptor(entityType, entityId, changes);
        }

        public static ChangesDescriptor Create<TEntity>(TEntity entity, IReadOnlyDictionary<string, PropertyChangeDescriptor> changes) where TEntity : class, IEntity
        {
            var entityId = entity.GetId();
            var entityType = typeof(TEntity).AsEntityName();
            return new ChangesDescriptor(entityType, entityId, changes);
        }

        public static ChangesDescriptor Create<TEntity, TValue>(
                TEntity entity,
                Expression<Func<TEntity, TValue>> propertyExpression,
                TValue propertyOriginalValue,
                TValue propertyModifiedValue)
            where TEntity : class, IEntity
        {
            return Create(entity, ActionLog.Diff.ForProperty(propertyExpression, propertyOriginalValue, propertyModifiedValue));
        }

        public static ChangesDescriptor Create(Type entityType, long entityId, IReadOnlyDictionary<string, PropertyChangeDescriptor> changes)
        {
            return new ChangesDescriptor(entityType.AsEntityName(), entityId, changes);
        }

        public static ChangesDescriptor Create(IEntityType entityType, long entityId, IReadOnlyDictionary<string, PropertyChangeDescriptor> changes)
        {
            return new ChangesDescriptor(entityType, entityId, changes);
        }
    }
}
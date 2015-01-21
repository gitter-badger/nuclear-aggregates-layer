using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.API.Core.ActionLogging
{
    public static class ActionLog
    {
        public static Dictionary<string, PropertyChangeDescriptor> Diff
        {
            get { return new Dictionary<string, PropertyChangeDescriptor>(); }
        }

        public static Dictionary<string, PropertyChangeDescriptor> ForProperty<TEntity, TValue>(
                this Dictionary<string, PropertyChangeDescriptor> differences,
                Expression<Func<TEntity, TValue>> propertyExpression,
                TValue propertyOriginalValue,
                TValue propertyModifiedValue)
            where TEntity : class, IEntity
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            differences.Add(propertyName, new PropertyChangeDescriptor(propertyOriginalValue, propertyModifiedValue));
            return differences;
        }

        public static void LogChanges<TEntity>(this IActionLogger actionLogger, 
                TEntity originalObject,
                TEntity modifiedObject)
            where TEntity : class, IEntity
        {
            actionLogger.LogChanges(ChangesDescriptor.Create(originalObject, modifiedObject));
        }

        public static void LogChanges<TEntity, TValue>(this IActionLogger actionLogger, 
                TEntity entity,
                Expression<Func<TEntity, TValue>> propertyExpression,
                TValue propertyOriginalValue,
                TValue propertyModifiedValue)
            where TEntity : class, IEntity
        {
            actionLogger.LogChanges(ChangesDescriptor.Create(entity, Diff.ForProperty(propertyExpression, propertyOriginalValue, propertyModifiedValue)));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.API.Core.ActionLogging
{
    public static class ActionLog
    {
        public static IDictionary<string, Tuple<object, object>> Diff
        {
            get { return new Dictionary<string, Tuple<object, object>>(); }
        }

        public static IDictionary<string, Tuple<object, object>> ForProperty<TEntity>(
                this IDictionary<string, Tuple<object, object>> differences,
                Expression<Func<TEntity, object>> propertyExpression, 
                object propertyOriginalValue, 
                object propertyModifiedValue)
            where TEntity : class, IEntity
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            differences.Add(propertyName, new Tuple<object, object>(propertyOriginalValue, propertyModifiedValue));
            return differences;
        }

        public static void LogChanges<TEntity>(this IActionLogger actionLogger, 
                long entityId,
                Expression<Func<TEntity, object>> propertyExpression,
                object propertyOriginalValue,
                object propertyModifiedValue)
            where TEntity : class, IEntity
        {
            actionLogger.LogChanges(typeof(TEntity).AsEntityName(), entityId, Diff.ForProperty(propertyExpression, propertyOriginalValue, propertyModifiedValue));
        }
    }
}
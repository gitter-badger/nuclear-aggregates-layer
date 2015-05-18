using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class PartableEntityAccessor
    {
        public static PartableValues<IEntityPart> Within(this IPartable entity, Type partType)
        {
            var part = entity.Parts.SingleOrDefault(x => x.GetType() == partType);
            return new PartableValues<IEntityPart>(part);
        }

        public static PartableValues<TTarget> Within<TTarget>(this IPartable entity) where TTarget : class, IEntityPart
        {
            var part = entity.Parts.OfType<TTarget>().SingleOrDefault();
            return new PartableValues<TTarget>(part);
        }

        public static IEnumerable<PartableValues<IEntityPart>> Within(this IPartable entity)
        {
            return entity.Parts.Select(x => new PartableValues<IEntityPart>(x));
        }

        public static void SyncPropertyValue<TProperty>(this IEnumerable<PartableValues<IEntityPart>> partableValues,
                                                        Expression<Func<IEntityPart, TProperty>> getter,
                                                        IPartable sourceEntity)
        {
            foreach (var partableValue in partableValues)
            {
                partableValue.SetPropertyValue(getter, sourceEntity.Within(partableValue.EntityPartType).GetPropertyValue(getter));
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class PartableValues<TTarget> where TTarget : class, IEntityPart
    {
        private readonly TTarget _entityPart;

        internal PartableValues(TTarget entityPart)
        {
            _entityPart = entityPart;
        }

        internal Type EntityPartType
        {
            get { return _entityPart.GetType(); }
        }

        public TProperty GetPropertyValue<TProperty>(Expression<Func<TTarget, TProperty>> getter)
        {
            return _entityPart != null ? _entityPart.GetPropertyValue(getter) : default(TProperty);
        }

        public void SetPropertyValue<TProperty>(Expression<Func<TTarget, TProperty>> getter, TProperty value)
        {
            if (_entityPart == null)
            {
                return;
            }

            _entityPart.SetPropertyValue(getter, value);
        }
    }
}
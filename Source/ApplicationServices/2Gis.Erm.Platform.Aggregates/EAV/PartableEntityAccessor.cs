using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Aggregates.EAV
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class PartableEntityAccessor
    {
        public static PartableValues<TTarget> Within<TTarget>(this IPartable entity) where TTarget : class, IEntityPart
        {
            var part = ExtractPart<TTarget>(entity);
            return new PartableValues<TTarget>(part);
        }

        private static TPart ExtractPart<TPart>(IPartable entity) where TPart : IEntityPart
        {
            // COMMENT {v.lapeev, 19.05.2014}: Cast использовать нельзя, т.к. потенциально может быть много дополнений разных типов
            return entity.Parts.OfType<TPart>().SingleOrDefault();
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
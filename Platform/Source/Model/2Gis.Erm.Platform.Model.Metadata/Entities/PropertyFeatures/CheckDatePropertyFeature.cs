using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class CheckDatePropertyFeature : IValidatablePropertyFeature
    {
        public CheckDatePropertyFeature(CheckDayOfMonthType checkDayOfMonthType, Type resourceManagerType, string resourceKey)
        {
            CheckDayOfMonthType = checkDayOfMonthType;
            ErrorMessageResourceManagerType = resourceManagerType;
            ResourceKey = resourceKey;
        }

        public CheckDayOfMonthType CheckDayOfMonthType
        {
            get;
            private set;
        }

        public Type ErrorMessageResourceManagerType { get; private set; }

        public string ResourceKey { get; private set; }

        public static CheckDatePropertyFeature Create<TKey>(CheckDayOfMonthType checkDayOfMonthType, Expression<Func<TKey>> errorMessageResourceKeyExpression)
        {
            var errorMessageKeyName = StaticReflection.GetMemberName(errorMessageResourceKeyExpression);
            var errorMessageResourceManagerType = StaticReflection.GetMemberDeclaringType(errorMessageResourceKeyExpression);
            return new CheckDatePropertyFeature(checkDayOfMonthType, errorMessageResourceManagerType, errorMessageKeyName);
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}

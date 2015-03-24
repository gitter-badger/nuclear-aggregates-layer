using System;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Resources.Server;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class DigitsOnlyPropertyFeature : IValidatablePropertyFeature
    {
        public Type ErrorMessageResourceManagerType
        {
            get { return StaticReflection.GetMemberDeclaringType(() => ResPlatform.OnlyDigitsAttributeValidationMessage); }
        }

        public string ResourceKey
        {
            get { return StaticReflection.GetMemberName(() => ResPlatform.OnlyDigitsAttributeValidationMessage); }
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}

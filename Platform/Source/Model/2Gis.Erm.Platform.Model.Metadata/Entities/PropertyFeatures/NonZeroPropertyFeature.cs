using System;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Resources.Server;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class NonZeroPropertyFeature : IValidatablePropertyFeature
    {
        public Type ErrorMessageResourceManagerType { get { return StaticReflection.GetMemberDeclaringType(() => ResPlatform.InappropriateValueForField); } }

        public string ResourceKey { get { return StaticReflection.GetMemberName(() => ResPlatform.InappropriateValueForField); } }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}

using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class NonZeroPropertyFeature : IValidatablePropertyFeature
    {
        public Type ErrorMessageResourceManagerType { get { return StaticReflection.GetMemberDeclaringType(() => BLResources.InappropriateValueForField); } }

        public string ResourceKey { get { return StaticReflection.GetMemberName(() => BLResources.InappropriateValueForField); } }

        public EntityProperty TargetProperty { get; set; }
    }
}

using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class RequiredPropertyFeature : IValidatablePropertyFeature
    {
        public Type ErrorMessageResourceManagerType { get { return StaticReflection.GetMemberDeclaringType(() => BLResources.RequiredFieldMessage); } }

        public string ResourceKey { get { return StaticReflection.GetMemberName(() => BLResources.RequiredFieldMessage); } }

        public EntityProperty TargetProperty { get; set; }
    }
}

using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class EmailPropertyFeature : IValidatablePropertyFeature
    {
        public Type ErrorMessageResourceManagerType { get { return StaticReflection.GetMemberDeclaringType(() => BLResources.InputValidationInvalidEmail); } }

        public string ResourceKey { get { return StaticReflection.GetMemberName(() => BLResources.InputValidationInvalidEmail); } }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}

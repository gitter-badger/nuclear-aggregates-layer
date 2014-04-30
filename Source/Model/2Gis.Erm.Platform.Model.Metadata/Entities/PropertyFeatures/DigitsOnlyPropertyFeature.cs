﻿using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class DigitsOnlyPropertyFeature : IValidatablePropertyFeature
    {
        public Type ErrorMessageResourceManagerType { get { return StaticReflection.GetMemberDeclaringType(() => BLResources.OnlyDigitsAttributeValidationMessage); } }

        public string ResourceKey { get { return StaticReflection.GetMemberName(() => BLResources.OnlyDigitsAttributeValidationMessage); } }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}

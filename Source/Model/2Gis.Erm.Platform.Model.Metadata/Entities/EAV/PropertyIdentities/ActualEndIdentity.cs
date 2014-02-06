using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class ActualEndIdentity : EntityPropertyIdentityBase<ActualEndIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.ActualEndPropertyId; }
        }

        public override string Description
        {
            get { return "ActualEnd"; }
        }

        public override string PropertyName
        {
            get { return "ActualEnd"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime?); }
        }
    }
}
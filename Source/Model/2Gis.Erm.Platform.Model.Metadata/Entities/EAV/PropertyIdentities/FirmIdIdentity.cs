using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class FirmIdIdentity : EntityPropertyIdentityBase<FirmIdIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.FirmIdPropertyId; }
        }

        public override string Description
        {
			get { return PropertyName; }
        }

        public override string PropertyName
        {
			get { return "FirmId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(long?); }
        }
    }
}
using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class CommuneIdIdentity : EntityPropertyIdentityBase<CommuneIdIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.CommuneIdPropertyId; }
        }

        public override string Description
        {
            get { return "CommuneId"; }
        }

        public override string PropertyName
        {
            get { return "CommuneId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(long); }
        }
    }
}
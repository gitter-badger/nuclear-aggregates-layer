using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class OrganizationUnitIdIdentity : EntityPropertyIdentityBase<OrganizationUnitIdIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.OrganizationUnitIdPropertyId; }
        }

        public override string Description
        {
            get { return "OrganizationUnitId"; }
        }

        public override string PropertyName
        {
            get { return "OrganizationUnitId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(long); }
        }
    }
}
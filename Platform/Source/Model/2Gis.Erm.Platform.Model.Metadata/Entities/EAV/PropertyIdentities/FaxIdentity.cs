using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class FaxIdentity : EntityPropertyIdentityBase<FaxIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.FaxPropertyId; }
        }

        public override string Description
        {
            get { return "Fax"; }
        }

        public override string PropertyName
        {
            get { return "Fax"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
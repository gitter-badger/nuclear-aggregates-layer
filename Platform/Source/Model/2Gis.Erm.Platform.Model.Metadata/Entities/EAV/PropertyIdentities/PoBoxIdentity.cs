using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class PoBoxIdentity : EntityPropertyIdentityBase<PoBoxIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.PoBoxPropertyId; }
        }

        public override string Description
        {
            get { return "P.O.Box"; }
        }

        public override string PropertyName
        {
            get { return "PoBox"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
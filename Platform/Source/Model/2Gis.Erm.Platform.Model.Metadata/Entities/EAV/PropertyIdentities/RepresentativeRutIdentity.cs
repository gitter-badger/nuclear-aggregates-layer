using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class RepresentativeRutIdentity : EntityPropertyIdentityBase<RepresentativeRutIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.RutPropertyId; }
        }

        public override string Description
        {
            get { return "RepresentativeRut"; }
        }

        public override string PropertyName
        {
            get { return "RepresentativeRut"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
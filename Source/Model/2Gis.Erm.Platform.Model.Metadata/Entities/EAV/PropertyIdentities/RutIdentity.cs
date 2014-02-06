using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class RutIdentity : EntityPropertyIdentityBase<RutIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.RutPropertyId; }
        }

        public override string Description
        {
            get { return "Rut"; }
        }

        public override string PropertyName
        {
            get { return "Rut"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
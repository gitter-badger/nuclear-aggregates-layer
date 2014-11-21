using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class MfoIdentity : EntityPropertyIdentityBase<MfoIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.MfoPropertyId; }
        }

        public override string Description
        {
            get { return "код межфилиальных оборотов"; }
        }

        public override string PropertyName
        {
            get { return "Mfo"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}

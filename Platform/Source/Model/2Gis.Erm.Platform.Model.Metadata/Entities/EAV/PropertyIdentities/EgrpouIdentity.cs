using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class EgrpouIdentity : EntityPropertyIdentityBase<EgrpouIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.EgrpouPropertyId; }
        }

        public override string Description
        {
            get { return "Egrpou"; }
        }

        public override string PropertyName
        {
            get { return "Egrpou"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
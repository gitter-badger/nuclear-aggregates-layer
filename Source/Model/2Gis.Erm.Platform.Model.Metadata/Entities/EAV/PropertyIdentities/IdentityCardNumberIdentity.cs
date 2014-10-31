using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class IdentityCardNumberIdentity : EntityPropertyIdentityBase<IdentityCardNumberIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IdentityCardNumberPropertyId; }
        }

        public override string Description
        {
            get { return "Номер удостоверения личности"; }
        }

        public override string PropertyName
        {
            get { return "IdentityCardNumber"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
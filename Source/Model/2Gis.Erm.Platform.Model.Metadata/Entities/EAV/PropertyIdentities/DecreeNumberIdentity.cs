using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class DecreeNumberIdentity : EntityPropertyIdentityBase<DecreeNumberIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.DecreeNumberPropertyId; }
        }

        public override string Description
        {
            get { return "Номер приказа"; }
        }

        public override string PropertyName
        {
            get { return "DecreeNumber"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
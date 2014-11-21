using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class RnnIdentity : EntityPropertyIdentityBase<RnnIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.RnnPropertyId; }
        }

        public override string Description
        {
            get { return "Rnn"; }
        }

        public override string PropertyName
        {
            get { return "Rnn"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
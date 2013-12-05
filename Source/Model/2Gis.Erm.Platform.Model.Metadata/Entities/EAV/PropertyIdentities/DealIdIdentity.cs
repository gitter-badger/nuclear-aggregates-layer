using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class DealIdIdentity : EntityPropertyIdentityBase<DealIdIdentity>
    {
        public override int Id
        {
            get { return 10; }
        }

        public override string Description
        {
            get { return "DealId"; }
        }

        public override string PropertyName
        {
            get { return "DealId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(int); }
        }
    }
}
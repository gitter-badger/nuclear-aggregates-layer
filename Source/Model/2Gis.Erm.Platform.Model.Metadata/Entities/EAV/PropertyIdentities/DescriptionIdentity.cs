using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class DescriptionIdentity : EntityPropertyIdentityBase<DescriptionIdentity>
    {
        public override int Id
        {
            get { return 8; }
        }

        public override string Description
        {
            get { return "Description"; }
        }

        public override string PropertyName
        {
            get { return "Description"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
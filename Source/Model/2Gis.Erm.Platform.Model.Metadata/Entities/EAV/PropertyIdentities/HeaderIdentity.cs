using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class HeaderIdentity : EntityPropertyIdentityBase<HeaderIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.HeaderPropertyId; }
        }

        public override string Description
        {
            get { return "Header"; }
        }

        public override string PropertyName
        {
            get { return "Header"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
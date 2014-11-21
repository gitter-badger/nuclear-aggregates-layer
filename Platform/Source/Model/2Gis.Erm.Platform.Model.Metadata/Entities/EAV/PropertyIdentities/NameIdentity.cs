using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class NameIdentity : EntityPropertyIdentityBase<NameIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.NamePropertyId; }
        }

        public override string Description
        {
            get { return "Name"; }
        }

        public override string PropertyName
        {
            get { return "Name"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
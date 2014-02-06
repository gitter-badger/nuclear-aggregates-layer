using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class IssuedByIdentity : EntityPropertyIdentityBase<IssuedByIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IssuedByPropertyId; }
        }

        public override string Description
        {
            get { return "IssuedBy"; }
        }

        public override string PropertyName
        {
            get { return "IssuedBy"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class BusinessModelIdentity : EntityPropertyIdentityBase<BusinessModelIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.BusinessModelPropertyId; }
        }

        public override string Description
        {
            get { return "BusinessModel"; }
        }

        public override string PropertyName
        {
            get { return "BusinessModel"; }
        }

        public override Type PropertyType
        {
            get { return typeof(BusinessModel); }
        }
    }
}
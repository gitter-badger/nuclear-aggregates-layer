using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class DecreeDateIdentity : EntityPropertyIdentityBase<DecreeDateIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.DecreeDatePropertyId; }
        }

        public override string Description
        {
            get { return "Дата приказа"; }
        }

        public override string PropertyName
        {
            get { return "DecreeDate"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime?); }
        }
    }
}
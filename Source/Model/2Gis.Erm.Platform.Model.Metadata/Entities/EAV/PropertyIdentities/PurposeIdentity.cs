using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class PurposeIdentity : EntityPropertyIdentityBase<PurposeIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.PurposePropertyId; }
        }

        public override string Description 
        {
            get { return "Purpose"; }
        }

        public override string PropertyName
        {
            get { return "Purpose"; }
        }

        public override Type PropertyType
        {
            get { return typeof(ActivityPurpose); }
        }
    }
}
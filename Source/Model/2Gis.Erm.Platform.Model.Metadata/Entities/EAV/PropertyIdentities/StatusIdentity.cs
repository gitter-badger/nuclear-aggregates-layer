using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class StatusIdentity : EntityPropertyIdentityBase<StatusIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.StatusPropertyId; }
        }

        public override string Description
        {
            get { return "Status"; }
        }

        public override string PropertyName
        {
            get { return "Status"; }
        }

        public override Type PropertyType
        {
            get { return typeof(ActivityStatus); }
        }
    }
}
using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class IssuedOnIdentity : EntityPropertyIdentityBase<IssuedOnIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IssuedOnPropertyId; }
        }

        public override string Description
        {
            get { return "IssuedOn"; }
        }

        public override string PropertyName
        {
            get { return "IssuedOn"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime); }
        }
    }
}
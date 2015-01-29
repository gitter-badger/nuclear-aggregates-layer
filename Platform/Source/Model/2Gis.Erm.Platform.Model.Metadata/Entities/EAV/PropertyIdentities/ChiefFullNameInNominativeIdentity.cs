using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class ChiefFullNameInNominativeIdentity : EntityPropertyIdentityBase<ChiefFullNameInNominativeIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.ChiefFullNameInNominativeIdentityPropertyId; }
        }

        public override string Description
        {
            get { return "ChiefFullNameInNominativeIdentity"; }
        }

        public override string PropertyName
        {
            get { return "ChiefFullNameInNominative"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
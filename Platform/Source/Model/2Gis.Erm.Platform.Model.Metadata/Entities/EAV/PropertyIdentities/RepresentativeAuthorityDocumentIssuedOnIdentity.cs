using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class RepresentativeAuthorityDocumentIssuedOnIdentity : EntityPropertyIdentityBase<RepresentativeAuthorityDocumentIssuedOnIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IssuedOnPropertyId; }
        }

        public override string Description
        {
            get { return "RepresentativeAuthorityDocumentIssuedOn"; }
        }

        public override string PropertyName
        {
            get { return "RepresentativeAuthorityDocumentIssuedOn"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime?); }
        }
    }
}
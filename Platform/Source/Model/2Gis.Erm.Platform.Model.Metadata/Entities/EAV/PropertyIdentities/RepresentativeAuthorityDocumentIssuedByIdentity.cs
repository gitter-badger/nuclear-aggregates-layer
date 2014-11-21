using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class RepresentativeAuthorityDocumentIssuedByIdentity : EntityPropertyIdentityBase<RepresentativeAuthorityDocumentIssuedByIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IssuedByPropertyId; }
        }

        public override string Description
        {
            get { return "RepresentativeAuthorityDocumentIssuedBy"; }
        }

        public override string PropertyName
        {
            get { return "RepresentativeAuthorityDocumentIssuedBy"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
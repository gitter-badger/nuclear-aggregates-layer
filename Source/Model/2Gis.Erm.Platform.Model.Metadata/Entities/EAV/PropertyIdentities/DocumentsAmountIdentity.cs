using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class DocumentsAmountIdentity : EntityPropertyIdentityBase<DocumentsAmountIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.DocumentsAmountPropertyId; }
        }

        public override string Description
        {
            get { return "DocumentsAmount"; }
        }

        public override string PropertyName
        {
            get { return "DocumentsAmount"; }
        }

        public override Type PropertyType
        {
            get { return typeof(int); }
        }
    }
}
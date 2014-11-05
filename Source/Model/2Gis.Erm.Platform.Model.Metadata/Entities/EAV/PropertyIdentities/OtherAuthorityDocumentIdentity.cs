using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class OtherAuthorityDocumentIdentity : EntityPropertyIdentityBase<OtherAuthorityDocumentIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.OtherAuthorityDocumentPropertyId; }
        }

        public override string Description
        {
            get { return "Поле для описания значения 'Действует на основании (род.падеж)' = 'Другое'"; }
        }

        public override string PropertyName
        {
            get { return "OtherAuthorityDocument"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
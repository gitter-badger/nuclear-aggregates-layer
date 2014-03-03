using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class OperationsKindIndentity : EntityPropertyIdentityBase<OperationsKindIndentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.OperationsKindPropertyId; }
        }

        public override string Description
        {
            get { return "OperationsKind"; }
        }

        public override string PropertyName
        {
            get { return "OperationsKind"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
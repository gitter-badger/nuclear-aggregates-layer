using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class MsCrmExportIdentity : OperationIdentityBase<MsCrmExportIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.MsCrmExportIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Export to Dynamics CRM";
            }
        }
    }
}
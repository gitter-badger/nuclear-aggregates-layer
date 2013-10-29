using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class ExportIdentity : OperationIdentityBase<ExportIdentity>, IEntitySpecificOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ExportIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Export";
            }
        }
    }
}
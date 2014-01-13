using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.UI.Metadata.Operations;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic
{
    public class CloseIdentity : OperationIdentityBase<CloseIdentity>, IClientOperationIdentity
    {
        public override int Id
        {
            get { return ClientOperationIdentityIds.CloseIdentity; }
        }

        public override string Description
        {
            get { return "Close element"; }
        }
    }
}

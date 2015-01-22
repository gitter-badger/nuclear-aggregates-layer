using DoubleGis.Erm.Platform.UI.Metadata.Operations;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic
{
    public class CloseIdentity : OperationIdentityBase<CloseIdentity>, INonCoupledOperationIdentity, IClientOperationIdentity
    {
        public override int Id
        {
            get { return ClientOperationIdentityIds.CloseIdentity; }
        }

        public override string Description
        {
            get { return "Close action (Close dialog, window, view and etc.)"; }
        }
    }
}

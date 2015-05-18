using System;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    [Obsolete]
    public sealed class RemoveBargainIdentity : OperationIdentityBase<RemoveBargainIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.RemoveBargainIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "RemoveBargain";
            }
        }
    }
}
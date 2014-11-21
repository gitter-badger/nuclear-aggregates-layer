﻿using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class ActualizeAccountsDuringWithdrawalIdentity : OperationIdentityBase<ActualizeAccountsDuringWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeAccountsDuringWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Actualize accounts state during withdrawal process, modify accounts, locks etc"; }
        }
    }
}

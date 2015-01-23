﻿using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal
{
    [DataContract]
    public sealed class ChangeDealStageIdentity : OperationIdentityBase<ChangeDealStageIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ChangeDealStageIdentity; }
        }

        public override string Description
        {
            get { return "Change deal stage."; }
        }
    }
}

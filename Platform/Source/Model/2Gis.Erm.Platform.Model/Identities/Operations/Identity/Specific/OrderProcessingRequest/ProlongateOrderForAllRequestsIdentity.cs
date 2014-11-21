﻿using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest
{
    // 2+ \Platform\Source\Model\2Gis.Erm.Platform.Model\Identities\Operations\Identity\Specific\OrderProcessingRequest
    [DataContract]
    public sealed class ProlongateOrderForAllRequestsIdentity : OperationIdentityBase<ProlongateOrderForAllRequestsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ProlongateOrderForAllRequestsIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Prolongate orders massive";
            }
        }
    }
}

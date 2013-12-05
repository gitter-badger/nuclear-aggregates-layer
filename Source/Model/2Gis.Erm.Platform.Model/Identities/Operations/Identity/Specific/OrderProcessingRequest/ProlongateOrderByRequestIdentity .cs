﻿using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest
{
    [DataContract]
    public sealed class ProlongateOrderByRequestIdentity : OperationIdentityBase<ProlongateOrderByRequestIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ProlongateOrderByRequestIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Prolongate order";
            }
        }
    }
}

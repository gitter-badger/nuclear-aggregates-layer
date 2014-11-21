﻿using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price
{
    [DataContract]
    public sealed class CopyPricePositionIdentity : OperationIdentityBase<CopyPricePositionIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CopyPricePositionIdentity; }
        }

        public override string Description
        {
            get { return "Copy price position"; }
        }
    }
}
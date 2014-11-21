﻿using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class PrintBillRequest: Request
    {
        public long Id { get; set; }
        public long? LegalPersonProfileId { get; set; }
    }
}

using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bargains
{
    public interface ICloseClientBargainsOperationService : IOperation<BulkCloseClientBargainsIdentity>
    {
        CloseBargainsResult CloseClientBargains(DateTime closeDate);
    }
}
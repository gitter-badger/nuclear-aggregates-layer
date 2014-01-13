using System;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AfterSaleService;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AfterSaleService
{
    public interface IUpdateAfterSaleServiceOperationService : IOperation<UpdateAfterSaleServiceIdentity>
    {
        void Update(Guid dealReplicationCode, DateTime activityDate, AfterSaleServiceType afterSaleServiceType);
    }
}

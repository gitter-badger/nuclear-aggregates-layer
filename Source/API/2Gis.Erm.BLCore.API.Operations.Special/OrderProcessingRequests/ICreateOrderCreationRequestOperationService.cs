using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations.Special\OrderProcessingRequests
    public interface ICreateOrderCreationRequestOperationService : IOperation<RequestOrderCreationIdentity>
    {
        long CreateOrderRequest(long sourceProjectCode,
                                DateTime beginDistributionDate,
                                short releaseCountPlan,
                                long firmId,
                                long legalPersonProfileId,
                                string description);
    }
}

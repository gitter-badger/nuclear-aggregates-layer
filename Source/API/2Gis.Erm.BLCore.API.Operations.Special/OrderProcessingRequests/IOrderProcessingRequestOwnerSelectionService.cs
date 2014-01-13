using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {d.ivanov, 28.11.2013}: IReadOnlyModel
    // TODO {d.ivanov, 28.11.2013}: ляжет в 2Gis.Erm.BLCore.Aggregates\Users\ReadModel\IOrderProcessingRequestOwnerSelectionService.cs
    public interface IOrderProcessingRequestOwnerSelectionService : ISimplifiedModelConsumer
    {
        User GetOwner(long userId);
        User GetOrganizationUnitDirector(long organizationUnitId);
        User GetReserveUser();
    }
}

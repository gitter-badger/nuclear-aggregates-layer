using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Advertisement
{
    public class AdvertisementReadModelTest : IIntegrationTest
    {
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IAppropriateEntityProvider<OrderPosition> _orderPositionProvider;
        private readonly IAppropriateEntityProvider<Position> _positionProvider;

        public AdvertisementReadModelTest(IAdvertisementReadModel advertisementReadModel,
                                          IAppropriateEntityProvider<OrderPosition> orderPositionProvider,
                                          IAppropriateEntityProvider<Position> positionProvider)
        {
            _advertisementReadModel = advertisementReadModel;
            _orderPositionProvider = orderPositionProvider;
            _positionProvider = positionProvider;
        }

        public ITestResult Execute()
        {
            var orderPosition =
                _orderPositionProvider.Get(Specs.Find.ActiveAndNotDeleted<OrderPosition>() &&
                                           new FindSpecification<OrderPosition>(
                                               op =>
                                               op.PricePosition.Position.MasterPositions.Any() && op.PricePosition.Position.ExportCode == 1 ||
                                               op.PricePosition.Position.ExportCode == 8));

            var position = _positionProvider.Get(Specs.Find.ActiveAndNotDeleted<Position>() &&
                                                 new FindSpecification<Position>(p => p.PricePositions.Any(pp => pp.Id == orderPosition.PricePositionId)));

            if (orderPosition == null || position == null)
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var orderPositionInfo = new OrderPositionInfo
                {
                    Id = orderPosition.Id,
                    AdvertisingMaterials = new[]
                        {
                            new AdvertisingMaterialInfo { StableRubrIds = Enumerable.Empty<long>() }
                        },
                    ProductType = position.ExportCode
                };

            _advertisementReadModel.Convert(orderPositionInfo);

            return orderPositionInfo.AdvertisingMaterials.All(x => x.StableRubrIds.Any()) ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
        }
    }
}
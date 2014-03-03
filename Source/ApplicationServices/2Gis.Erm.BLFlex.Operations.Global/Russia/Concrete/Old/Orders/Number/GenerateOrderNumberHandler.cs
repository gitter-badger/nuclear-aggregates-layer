using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.Number
{
    public sealed class GenerateOrderNumberHandler : RequestHandler<GenerateOrderNumberRequest, GenerateOrderNumberResponse>, IRussiaAdapted
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly OrderNumberGenerationStrategy[] _orderNumberGenerationStrategies;

        public GenerateOrderNumberHandler(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
            _orderNumberGenerationStrategies = new OrderNumberGenerationStrategy[]
                {
                    OrderNumberGenerationStrategies.Russia.ReadFromNewFormat,
                    OrderNumberGenerationStrategies.Russia.ReadFromOldFormat,
                    OrderNumberGenerationStrategies.UseReservedNumber,
                    OrderNumberGenerationStrategies.UseExistingOrderNumber
                };
        }

        protected override GenerateOrderNumberResponse Handle(GenerateOrderNumberRequest request)
        {
            var order = request.Order;
            var syncCodes = _orderReadModel.GetOrderOrganizationUnitsSyncCodes(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);
            var numberBillet = request.IsRegionalNumber ? "ОФ_{0}-{1}-{2}" : "БЗ_{0}-{1}-{2}";
            var numberFormat = string.Format(numberBillet, syncCodes[order.SourceOrganizationUnitId], syncCodes[order.DestOrganizationUnitId], "{0}");
            string orderNumber = null;

            foreach (var strategy in _orderNumberGenerationStrategies)
            {
                if (strategy(order.Number, numberFormat, request.ReservedNumber, out orderNumber))
                {
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                throw new NotificationException(BLResources.FailedToGenerateOrderNumber);
            }

            return new GenerateOrderNumberResponse { Number = orderNumber };
        }
    }
}
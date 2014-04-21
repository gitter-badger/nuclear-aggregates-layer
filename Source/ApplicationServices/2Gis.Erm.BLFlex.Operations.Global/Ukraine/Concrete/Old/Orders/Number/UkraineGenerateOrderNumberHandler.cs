using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Orders.Number
{
    public sealed class UkraineGenerateOrderNumberHandler : RequestHandler<GenerateOrderNumberRequest, GenerateOrderNumberResponse>, IUkraineAdapted
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly OrderNumberGenerationStrategy[] _orderNumberGenerationStrategies;

        public UkraineGenerateOrderNumberHandler(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
            _orderNumberGenerationStrategies = new OrderNumberGenerationStrategy[]
                {
                    OrderNumberGenerationStrategies.MultiCulture.ReadFromNewFormat,
                    OrderNumberGenerationStrategies.MultiCulture.ReadFromOldFormat,
                    OrderNumberGenerationStrategies.UseReservedNumber,
                    OrderNumberGenerationStrategies.UseExistingOrderNumber
                };
        }

        protected override GenerateOrderNumberResponse Handle(GenerateOrderNumberRequest request)
        {
            var order = request.Order;
            var syncCodes = _orderReadModel.GetOrderOrganizationUnitsSyncCodes(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);
            if (request.IsRegionalNumber)
            {
                throw new NotSupportedException("Regional orders not supported by business model");
            }

            var numberFormat = string.Format("��_{0}-{1}-{2}", syncCodes[order.SourceOrganizationUnitId], syncCodes[order.DestOrganizationUnitId], "{0}");
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
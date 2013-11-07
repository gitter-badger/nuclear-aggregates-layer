using System;

using DoubleGis.Erm.BL.Aggregates.Orders;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Operations.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: �������� �� BL.Operations - ��� ����� � ������ �������, ������ �� ������������ ������ � TFS ��-�� �������������� merge - ���� ��������� ��� �����, ��� RI �� 1.0 ����� �������� �������� ����� ������� ���� ���������� �� 2��
    // ������ ����������� ������� internal, ����� �� ������������� massprocessor
    internal sealed class CzechGenerateOrderNumberHandler : RequestHandler<GenerateOrderNumberRequest, GenerateOrderNumberResponse>, ICzechAdapted
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderNumberGenerationStrategy[] _orderNumberGenerationStrategies;

        public CzechGenerateOrderNumberHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _orderNumberGenerationStrategies = new OrderNumberGenerationStrategy[]
                {
                    OrderNumberGenerationStrategies.Czech.ReadFromNewFormat,
                    OrderNumberGenerationStrategies.Czech.ReadFromOldFormat,
                    OrderNumberGenerationStrategies.UseReservedNumber,
                    OrderNumberGenerationStrategies.UseExistingOrderNumber
                };
        }

        protected override GenerateOrderNumberResponse Handle(GenerateOrderNumberRequest request)
        {
            var order = request.Order;
            var syncCodes = _orderRepository.GetOrderOrganizationUnitsSyncCodes(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);
            if (request.IsRegionalNumber)
            {
                throw new NotSupportedException("Regional orders not supported by business model");
            }

            var numberFormat = string.Format("OBJ_{0}-{1}-{2}", syncCodes[order.SourceOrganizationUnitId], syncCodes[order.DestOrganizationUnitId], "{0}");
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
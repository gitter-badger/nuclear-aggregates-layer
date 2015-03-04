using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class ValidateOrdersForReleaseOperationService : IValidateOrdersForReleaseOperationService
    {
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICommonLog _logger;

        public ValidateOrdersForReleaseOperationService(IClientProxyFactory clientProxyFactory,
            IOrderReadModel orderReadModel,
            ICommonLog logger)
        {
            _clientProxyFactory = clientProxyFactory;
            _orderReadModel = orderReadModel;
            _logger = logger;
        }

        public IEnumerable<ReleaseProcessingMessage> Validate(long organizationUnitId, TimePeriod period, bool isBeta)
        {
            _logger.InfoFormat("Starting orders validation for release by organization unit with id {0} for period {1} release is {2}", organizationUnitId, period, isBeta ? "beta" : "final");
            
            var orderValidationServiceProxy = _clientProxyFactory.GetClientProxy<IOrderValidationApplicationService, WSHttpBinding>();
            var validationResults = orderValidationServiceProxy.Execute(
                service => service.ValidateOrders(
                    isBeta ? ValidationType.PreReleaseBeta : ValidationType.PreReleaseFinal,
                    organizationUnitId,
                    period,
                    null,
                    false));

            var convertedResults = ConvertValidationMessages(validationResults.Messages);

            _logger.InfoFormat("Finished orders validation for release by organization unit with id {0} for period {1} release is {2}", organizationUnitId, period, isBeta ? "beta" : "final");

            return convertedResults;
        }

        private IEnumerable<ReleaseProcessingMessage> ConvertValidationMessages(IEnumerable<OrderValidationMessage> orderValidationMessages)
        {
            orderValidationMessages = orderValidationMessages.Where(x => x.Type == MessageType.Error || x.Type == MessageType.Warning).ToArray();

            var orderIds = orderValidationMessages.Select(x => x.OrderId).Distinct().ToArray();
            var validationAdditionalInfos = _orderReadModel.GetOrderValidationAdditionalInfos(orderIds);

            var validationInfosDictionary = new Dictionary<long, OrderValidationAdditionalInfo>(validationAdditionalInfos.Count());
            foreach (var info in validationAdditionalInfos)
            {
                validationInfosDictionary[info.Id] = info;
            }

            var resultList = new Collection<ReleaseProcessingMessage>();

            foreach (var message in orderValidationMessages)
            {
                OrderValidationAdditionalInfo addInfo;
                validationInfosDictionary.TryGetValue(message.OrderId, out addInfo);
                var converted = new ReleaseProcessingMessage
                {
                    OrderId = message.OrderId,
                    OrderNumber = message.OrderNumber,
                    IsBlocking = message.Type == MessageType.Error,
                    RuleCode = message.RuleCode.ToString(CultureInfo.InvariantCulture),
                    Message = ConvertValidationMessage(message, addInfo)
                };
                resultList.Add(converted);
            }

            return resultList;
        }

        private string ConvertValidationMessage(OrderValidationMessage message, OrderValidationAdditionalInfo additionalInfo)
        {
            if (additionalInfo == null)
            {
                return message.MessageText;
            }

            return string.Format(BLResources.OrderValidationMessageFormat,
                                 additionalInfo.SourceOrganizationUnitName,
                                 additionalInfo.DestOrganizationUnitName,
                                 additionalInfo.OwnerName,
                                 additionalInfo.FirmName,
                                 additionalInfo.LegalPersonName,
                                 message.MessageText);
        }
    }
}

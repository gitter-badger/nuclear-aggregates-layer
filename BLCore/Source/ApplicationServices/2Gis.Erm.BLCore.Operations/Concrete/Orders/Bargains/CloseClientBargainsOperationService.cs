using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bargains;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bargains;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Bargains
{
    public class CloseClientBargainsOperationService : ICloseClientBargainsOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICloseClientBargainsAggregateService _closeClientBargainsAggregateService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public CloseClientBargainsOperationService(IOrderReadModel orderReadModel,
                                                   ICloseClientBargainsAggregateService closeClientBargainsAggregateService,
                                                   ISecurityServiceFunctionalAccess functionalAccessService,
                                                   IUserContext userContext)
        {
            _orderReadModel = orderReadModel;
            _closeClientBargainsAggregateService = closeClientBargainsAggregateService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public CloseBargainsResult CloseClientBargains(DateTime closeDate)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CloseBargainOperationalPeriod, _userContext.Identity.Code))
            {
                // TODO {all, 10.07.2014}: Заиспользовать исключение об ограничении доступа к операции, когда оно приедет из ветки
                throw new NotificationException(BLResources.CloseBargains_AccessDenied);
            }

            var bargains = _orderReadModel.GetNonClosedClientBargains();

            var invalidPeriodBargainsNumbers = bargains.Where(x => x.SignedOn > closeDate).Select(x => x.Number).ToArray();

            var response = new CloseBargainsResult();
            if (invalidPeriodBargainsNumbers.Length > 0)
            {
                response.NonClosedBargainsNumbers = invalidPeriodBargainsNumbers;
            }

            _closeClientBargainsAggregateService.CloseBargains(bargains.Where(x => x.SignedOn <= closeDate), closeDate);

            return response;
        }
    }
}
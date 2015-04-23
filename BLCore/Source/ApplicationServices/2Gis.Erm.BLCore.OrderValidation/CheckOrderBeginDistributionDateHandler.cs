using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    // TODO {all, 23.03.2014}: request объявлен в BLCore.API.Operations
    public sealed class CheckOrderBeginDistributionDateHandler : RequestHandler<CheckOrderBeginDistributionDateRequest, EmptyResponse>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public CheckOrderBeginDistributionDateHandler(
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOrderReadModel orderReadModel)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _orderReadModel = orderReadModel;
        }

        protected override EmptyResponse Handle(CheckOrderBeginDistributionDateRequest request)
        {
            var orderDistributionInfo = _orderReadModel.GetOrderInfoToCheckOrderBeginDistributionDate(request.OrderId);
            
            if (orderDistributionInfo == null ||
                orderDistributionInfo.BeginDistributionDate != request.BeginDistributionDate ||
                orderDistributionInfo.SourceOrganizationUnitId != request.SourceOrganizationUnitId ||
                orderDistributionInfo.DestinationOrganizationUnitId != request.DestinationOrganizationUnitId)
            {   // т.е. заказ либо ещё не сохранен в БД, либо есть изменения в заказе, нужно проверить дату начала размещения
                var isRegionalOrder = request.SourceOrganizationUnitId != 0 &&
                                      request.DestinationOrganizationUnitId != 0 &&
                                      request.DestinationOrganizationUnitId != request.SourceOrganizationUnitId;
                ValidateBeginDistributionDate(request.BeginDistributionDate, isRegionalOrder);
            }

            return Response.Empty;
        }

        // Параметр isRegionalOrder не используется в методе, однако не стоит его убирать, т.к. на практике требования меняются раз в месяц
        private void ValidateBeginDistributionDate(DateTime beginDistributionDate, bool isRegionalOrder)
        {
            var nextMonthFirstDate = DateTime.Today.AddMonths(1).GetFirstDateOfMonth();
            var afterNextMonthFirstDate = DateTime.Today.AddMonths(2).GetFirstDateOfMonth();
            var nextYearThisMonthFirstDate = DateTime.Today.AddYears(1).GetFirstDateOfMonth();

            if (beginDistributionDate < nextMonthFirstDate)
            {   // т.е. начало размещения указано в текущем месяце, попытка оформить заказ задним числом
                var releaseAccessFunctionalPrivilege =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, _userContext.Identity.Code);
                if (!releaseAccessFunctionalPrivilege)
                {
                    throw new NotificationException(BLResources.IncorrectBeginDistributionDateMin);
                }

                return;
            }

            if (beginDistributionDate > nextYearThisMonthFirstDate)
            {   // начало размещения указано более позднее чем, тот же месяц что и сейчас в следующем году
                throw new NotificationException(BLResources.IncorrectBeginDistributionDateMax);
            }

            // дата начала размещения указана в диапазоне: от след.месяц этого года до этого же месяца но уже в след. году
            if (_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.OrderCreationForFuture, _userContext.Identity.Code))
            {   // есть привилегия, дополнительных проверок не требуется
                return;
            }

            if (beginDistributionDate > afterNextMonthFirstDate)
            {   // для местных и для региональных заказов дата начала размещения должна быть в след. месяце. либо через 1 месяц (остаток текущего + 1 полный, затем размещение)
                throw new NotificationException(BLResources.AccessDeniedCreateOrderForFuture);
            }
        }
    }
}
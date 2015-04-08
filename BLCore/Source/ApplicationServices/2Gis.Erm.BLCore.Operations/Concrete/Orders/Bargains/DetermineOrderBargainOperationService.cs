using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bargains;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Bargains
{
    public class DetermineOrderBargainOperationService : IDetermineOrderBargainOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public DetermineOrderBargainOperationService(IOrderReadModel orderReadModel,
                                                     IOperationScopeFactory scopeFactory,
                                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                                     IUserContext userContext)
        {
            _orderReadModel = orderReadModel;
            _scopeFactory = scopeFactory;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
        }

        public bool TryDetermineOrderBargain(long legalPersonId,
                                             long branchOfficeOrganizationUnitId,
                                             DateTime orderEndDistributionDate,
                                             out long bargainId,
                                             out string bargainNumber)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<DetermineOrderBargainIdentity>())
            {
                bargainNumber = string.Empty;
                bargainId = 0;
                var bargains = _orderReadModel.GetSuitableBargains(legalPersonId, branchOfficeOrganizationUnitId, orderEndDistributionDate);

                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, _userContext.Identity.Code))
                {
                    bargains = bargains.Where(x => x.BargainKind != BargainKind.Agent).ToArray();
                }

                // Сделаем подстановку только если договор один и он будет действовать до окончания размещения заказа
                if (bargains.Count() != 1)
                {
                    operationScope.Complete();
                    return false;
                }

                var possibleBargain = bargains.Single();

                if (possibleBargain.EndDate < orderEndDistributionDate)
                {
                    operationScope.Complete();
                    return false;
                }

                bargainId = possibleBargain.Id;
                bargainNumber = possibleBargain.Number;

                operationScope.Complete();
                return true;
            }
        }
    }
}
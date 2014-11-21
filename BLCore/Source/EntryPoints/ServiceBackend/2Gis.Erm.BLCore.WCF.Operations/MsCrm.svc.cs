using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AfterSaleService;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Remote.MsCrm;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public sealed class MsCrm2ErmApplicationService : IMsCrm2ErmApplicationService
    {
        private readonly IUpdateAfterSaleServiceOperationService _updateAfterSaleServiceOperationService;
        private readonly IReplicateDealStageOperationService _replicateDealStageOperationService;
        private readonly ICommonLog _logger;

        public MsCrm2ErmApplicationService(
            IUpdateAfterSaleServiceOperationService updateAfterSaleServiceOperationService,
            IReplicateDealStageOperationService replicateDealStageOperationService,
            IUserContext userContext,
            IResourceGroupManager resourceGroupManager,
            ICommonLog logger)
        {
            _updateAfterSaleServiceOperationService = updateAfterSaleServiceOperationService;
            _replicateDealStageOperationService = replicateDealStageOperationService;
            _logger = logger;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public void UpdateAfterSaleActivity(Guid dealReplicationCode, DateTime activityDate, AfterSaleServiceType afterSaleServiceType)
        {
            try
            {
                _updateAfterSaleServiceOperationService.Update(dealReplicationCode, activityDate, afterSaleServiceType);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(
                    ex,
                    "Can't update after sale activities. Deal replication code:{0}. Activity date: {1}. After sale service type: {2}",
                    dealReplicationCode,
                    activityDate,
                    afterSaleServiceType);
                throw new FaultException<MsCrm2ErmErrorDescription>(new MsCrm2ErmErrorDescription(ex.Message));
            }
        }

        public void ReplicateDealStage(Guid dealReplicationCode, MsCrmDealStage msCrmDealStage, string userDomainName)
        {
            try
            {
                _replicateDealStageOperationService.Replicate(dealReplicationCode, (DealStage)msCrmDealStage, userDomainName);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(
                    ex,
                    "Can't replicate deal stage. Deal replication code:{0}. Deal stage date: {1}. User domain name: {2}",
                    dealReplicationCode,
                    msCrmDealStage,
                    userDomainName);
                throw new FaultException<MsCrm2ErmErrorDescription>(new MsCrm2ErmErrorDescription(ex.Message));
            }
        }
    }
}

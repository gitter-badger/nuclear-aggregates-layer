﻿using System;
using System.Linq;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class FinishReleaseOperationService : IFinishReleaseOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IReleaseChangeStatusAggregateService _releaseChangeStatusAggregateService;
        private readonly IAccountBulkCreateLocksAggregateService _accountBulkCreateLocksAggregateService;
        private readonly IAccountBulkCloseLimitsAggregateService _accountBulkCloseLimitsAggregateService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public FinishReleaseOperationService(IOrderReadModel orderReadModel,
                                             IReleaseReadModel releaseReadModel,
                                             IAccountReadModel accountReadModel,
                                             IReleaseChangeStatusAggregateService releaseChangeStatusAggregateService,
                                             IAccountBulkCreateLocksAggregateService accountBulkCreateLocksAggregateService,
                                             IAccountBulkCloseLimitsAggregateService accountBulkCloseLimitsAggregateService,
                                             ISecurityServiceFunctionalAccess functionalAccessService,
                                             IUserContext userContext,
                                             IOperationScopeFactory scopeFactory,
                                             ICommonLog logger)
        {
            _orderReadModel = orderReadModel;
            _releaseReadModel = releaseReadModel;
            _accountReadModel = accountReadModel;
            _releaseChangeStatusAggregateService = releaseChangeStatusAggregateService;
            _accountBulkCreateLocksAggregateService = accountBulkCreateLocksAggregateService;
            _accountBulkCloseLimitsAggregateService = accountBulkCloseLimitsAggregateService;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // TODO {d.ivanov, 17.01.2014}: Текущая реализация Succeeded предполагает, что в случае ошибки статус сборки не будет изменен на Error.
        //                              Тем самым, ответственность за вызов операции Failed лежит сейчас на вызывающей стороне.
        //                              Лучше бы выставлять статус Error явно, если были исключительные ситуации
        // COMMENT {d.ivanov, 17.01.2014}: Из-за наличия возможности "подхватить" выполняющуюся сборку, процесс сборки в целом нарушен не будет
        public void Succeeded(long releaseId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<FinishReleaseIdentity>())
            {
                var releaseInfo = ResolveRelease(releaseId);

                _logger.InfoFormatEx("Finishing release with id {0} ou:{1} period: {2} and success state",
                                     releaseId,
                                     releaseInfo.OrganizationUnitName,
                                     releaseInfo.Period);
                if (releaseInfo.Release.IsBeta)
                {
                    _logger.InfoEx("Release type is beta, so no actions required in ERM accounts (locks, limits and etc.) state");
                }
                else
                {
                    _logger.InfoEx("Creating locks");
                    var orderInfos = _orderReadModel.GetOrderReleaseInfos(releaseInfo.Release.OrganizationUnitId, releaseInfo.Period);
                    _accountBulkCreateLocksAggregateService.Create(releaseInfo.Period, orderInfos);
                    _logger.InfoEx("Locks created");

                    _logger.InfoEx("Closing limits");
                    var limitsForRelease = _accountReadModel.GetLimitsForRelease(releaseInfo.Release.OrganizationUnitId, releaseInfo.Period);
                    var hungLimitsBeforeReleasePeriod = _accountReadModel.GetHungLimitsByOrganizationUnitForDate(releaseInfo.Release.OrganizationUnitId,
                                                                                                                 releaseInfo.Period.Start.GetPrevMonthFirstDate());
                    _accountBulkCloseLimitsAggregateService.Close(limitsForRelease.Union(hungLimitsBeforeReleasePeriod));
                    _logger.InfoEx("Limits closed");
                }

                _releaseChangeStatusAggregateService.Finished(releaseInfo.Release, ReleaseStatus.Success, string.Empty);
                _logger.InfoFormatEx("Finished release with id {0} and success state", releaseId);

                scope.Complete();
            }
        }

        // TODO {d.ivanov, 17.01.2014}: Текущая реализация Failed предполагает, что в случае ошибки статус сборки не будет изменен на Error.
        //                              То есть, по этому контракту мы заставляем вызывающую сторону вызвать Failed еще раз в надежде на успех
        //                              Лучше бы выставлять статус Error явно, если были исключительные ситуации
        // COMMENT {d.ivanov, 17.01.2014}: Из-за наличия возможности "подхватить" выполняющуюся сборку, процесс сборки в целом нарушен не будет
        public void Failed(long releaseId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<FinishReleaseIdentity>())
            {
                var releaseInfo = ResolveRelease(releaseId);

                _logger.InfoFormatEx("Finishing release with id {0} ou:{1} period: {2} and failed state, " +
                                     "because errors detected in the release process on the external side",
                                     releaseId,
                                     releaseInfo.OrganizationUnitName,
                                     releaseInfo.Period);
                _releaseChangeStatusAggregateService.Finished(releaseInfo.Release,
                                                              ReleaseStatus.Error,
                                                              "Can't completly finish release. Errors detected in the release process on the external side");
                _logger.InfoFormatEx("Finished release with id {0} ou:{1} period: {2} and failed state, " +
                                     "because errors detected in the release process on the external side",
                                     releaseId,
                                     releaseInfo.OrganizationUnitName,
                                     releaseInfo.Period);

                scope.Complete();
            }
        }

        private ReleaseInfoDto ResolveRelease(long releaseId)
        {
            // Текущая реализация определения identity текущего пользователя установит в IUserContext.Identity учетку пула приложений IIS, если безопасность выключена для endpoint
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, _userContext.Identity.Code))
            {
                throw new SecurityAccessDeniedException("User doesn't have sufficient privileges for finishing release");
            }

            var release = _releaseReadModel.GetReleaseInfo(releaseId);
            if (release == null)
            {
                var msg = "Can't find release entry for specified release id " + releaseId;
                _logger.ErrorEx(msg);
                throw new ArgumentException(msg);
            }

            var releaseInfo = new ReleaseInfoDto
                {
                    Release = release,
                    OrganizationUnitName = _releaseReadModel.GetOrganizationUnitName(release.OrganizationUnitId),
                    Period = new TimePeriod(release.PeriodStartDate, release.PeriodEndDate)
                };

            var currentReleaseStatus = (ReleaseStatus)release.Status;
            if (currentReleaseStatus != ReleaseStatus.InProgressWaitingExternalProcessing)
            {
                var message = string.Format("Can't finish release with id {0} and status {1} ou:{2} period: {3}. " +
                                            "To execute finish, release have to be in status: {4}",
                                            releaseId,
                                            currentReleaseStatus,
                                            releaseInfo.OrganizationUnitName,
                                            releaseInfo.Period,
                                            ReleaseStatus.InProgressWaitingExternalProcessing);
                _logger.ErrorEx(message);
                throw new InvalidOperationException(message);
            }

            return releaseInfo;
        }
    }
}
using System;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.Operations
{
    public sealed class ReleaseStartAggregateService : IReleaseStartAggregateService
    {
        private readonly IRepository<ReleaseInfo> _releaseRepository;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public ReleaseStartAggregateService(
            IRepository<ReleaseInfo> releaseRepository,
            IUserContext userContext,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _releaseRepository = releaseRepository;
            _userContext = userContext;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public ReleaseInfo Start(long organizationUnitId, TimePeriod period, bool isBeta, ReleaseStatus targetStatus)
        {
            if (targetStatus != ReleaseStatus.InProgressInternalProcessingStarted && targetStatus != ReleaseStatus.InProgressWaitingExternalProcessing)
            {
                throw new InvalidOperationException(string.Format("Invalid target release status after release start was specifed: {0}", targetStatus));
            }

            ReleaseInfo newRelease;
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ReleaseInfo>())
            {
                newRelease = new ReleaseInfo
                    {
                        StartDate = DateTime.UtcNow,
                        PeriodStartDate = period.Start,
                        PeriodEndDate = period.End,
                        OrganizationUnitId = organizationUnitId,
                        Status = (short)targetStatus,
                        IsBeta = isBeta,
                        OwnerCode = _userContext.Identity.Code,
                        IsActive = true
                    };

                _identityProvider.SetFor(newRelease);
                _releaseRepository.Add(newRelease);
                scope.Added<ReleaseInfo>(newRelease.Id);

                _releaseRepository.Save();
                scope.Complete();
            }

            return newRelease;
        }
    }
}
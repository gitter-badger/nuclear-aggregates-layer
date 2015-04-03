using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.Operations
{
    public sealed class ReleaseStartAggregateService : IReleaseStartAggregateService
    {
        private readonly BusinessModel _businessModel;
        private readonly IRepository<ReleaseInfo> _releaseRepository;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public ReleaseStartAggregateService(IBusinessModelSettings globalizationSettings,
                                            IRepository<ReleaseInfo> releaseRepository,
                                            IUserContext userContext,
                                            IIdentityProvider identityProvider,
                                            IOperationScopeFactory scopeFactory)
        {
            _businessModel = globalizationSettings.BusinessModel;
            _releaseRepository = releaseRepository;
            _userContext = userContext;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public ReleaseInfo Start(int countryCode, long organizationUnitId, TimePeriod period, bool isBeta, ReleaseStatus targetStatus)
        {
            var countryCodes = BusinessModelCountryMapping.GetCountryCodes(_businessModel);
            if (countryCodes.All(x => x != countryCode))
            {
                throw new InvalidOperationException(string.Format("Can't start releasing for organization unit with id {0} by period {1} is beta {2} " +
                                                                  "within business model '{3}'",
                                                                  organizationUnitId,
                                                                  period,
                                                                  isBeta,
                                                                  _businessModel));
            }

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
                        Status = targetStatus,
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
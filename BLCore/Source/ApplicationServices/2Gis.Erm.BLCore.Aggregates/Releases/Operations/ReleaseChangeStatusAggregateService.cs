using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.Operations
{
    public sealed class ReleaseChangeStatusAggregateService : IReleaseChangeStatusAggregateService
    {
        private readonly IRepository<ReleaseInfo> _releaseRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ReleaseChangeStatusAggregateService(IRepository<ReleaseInfo> releaseRepository, IOperationScopeFactory scopeFactory)
        {
            _releaseRepository = releaseRepository;
            _scopeFactory = scopeFactory;
        }

        public void InProgressInternalProcessingStarted(ReleaseInfo release, string changesDescription)
        {
            release.Comment = changesDescription;
            ChangeStatus(release, ReleaseStatus.InProgressWaitingExternalProcessing, ReleaseStatus.InProgressInternalProcessingStarted);
        }

        public void InProgressWaitingExternalProcessing(ReleaseInfo release)
        {
            ChangeStatus(release, ReleaseStatus.InProgressInternalProcessingStarted, ReleaseStatus.InProgressWaitingExternalProcessing);
        }

        public void Finished(ReleaseInfo release, ReleaseStatus targetStatus, string changesDescription)
        {
            var sourceStatus = release.Status;
            if ((targetStatus != ReleaseStatus.Error && targetStatus != ReleaseStatus.Success) ||
                (sourceStatus != ReleaseStatus.InProgressInternalProcessingStarted && sourceStatus != ReleaseStatus.InProgressWaitingExternalProcessing))
            {
                throw new ArgumentException(string.Format("Check specified source and target release statuses. Source: {0}. Target: {1}", sourceStatus, targetStatus));
            }

            release.Status = targetStatus;
            release.Comment = changesDescription;
            release.FinishDate = DateTime.UtcNow;

            UpdateRelease(release);
        }

        public void Reverting(ReleaseInfo release, string changesDescription)
        {
            release.Comment = changesDescription;
            ChangeStatus(release, ReleaseStatus.Success, ReleaseStatus.Reverting);
        }

        public void Reverted(ReleaseInfo release)
        {
            ChangeStatus(release, ReleaseStatus.Reverting, ReleaseStatus.Reverted);
        }

        public void SetPreviousStatus(ReleaseInfo release, ReleaseStatus previousStatus, string changesDescription)
        {
            release.Status = previousStatus;
            release.Comment = changesDescription;

            UpdateRelease(release);
        }

        private void ChangeStatus(ReleaseInfo release, ReleaseStatus validSourceStatus, ReleaseStatus targetStatus)
        {
            var sourceStatus = release.Status;
            if (sourceStatus != validSourceStatus)
            {
                throw new ArgumentException(string.Format("{0} status can be set if release in {1} status only. Current source status: {2}", targetStatus, validSourceStatus, sourceStatus));
            }

            release.Status = targetStatus;
            UpdateRelease(release);
        }

        private void UpdateRelease(ReleaseInfo release)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, ReleaseInfo>())
            {
                _releaseRepository.Update(release);
                scope.Updated<ReleaseInfo>(release.Id);

                _releaseRepository.Save();
                scope.Complete();
            }
        }
    }
}
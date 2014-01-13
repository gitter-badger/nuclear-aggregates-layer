using System;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

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

        public void ChangeStatus(ReleaseInfo release, ReleaseStatus targetStatus, string changesDescription)
        {
            release.Status = (short)targetStatus;
            release.Comment = changesDescription;

            ChangeStatus(release);
        }

        public void Finished(ReleaseInfo release, ReleaseStatus targetStatus, string changesDescription)
        {
            var sourceStatus = (ReleaseStatus)release.Status;
            if (sourceStatus != ReleaseStatus.InProgressWaitingExternalProcessing 
                || (targetStatus != ReleaseStatus.Error && targetStatus != ReleaseStatus.Success))
            {
                throw new ArgumentException("Check specified source and target release statuses");
            }

            release.Status = (short)targetStatus;
            release.Comment = changesDescription;
            release.FinishDate = DateTime.UtcNow;

            ChangeStatus(release);
        }

        private void ChangeStatus(ReleaseInfo release)
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
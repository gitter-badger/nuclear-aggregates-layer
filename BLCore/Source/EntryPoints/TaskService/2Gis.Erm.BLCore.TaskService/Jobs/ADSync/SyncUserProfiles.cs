﻿using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.UserProfiles;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.ADSync
{
    public sealed class SyncUserProfiles : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public SyncUserProfiles(ICommonLog logger, IPublicService publicService, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new SyncUserProfilesRequest());
        }
    }
}

using DoubleGis.Erm.BLCore.API.Operations.Generic.File;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class DeleteOrphanFilesJob : TaskServiceJobBase
    {
        private readonly IFileService _fileService;

        public DeleteOrphanFilesJob(
            ITracer tracer,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            IFileService fileService)
            : base(signInService, userImpersonationService, tracer)
        {
            _fileService = fileService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _fileService.DeleteOrhpanFiles();
        }
    }
}

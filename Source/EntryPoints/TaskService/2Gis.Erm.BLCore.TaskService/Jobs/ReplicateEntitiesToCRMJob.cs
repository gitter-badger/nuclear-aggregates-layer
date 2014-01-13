using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    // TODO {all, 05.08.2013}: Есть механизм обработчиков лога бизнес операций - скорее всего репликация в CRM должна инициироваться из такого обработчика. Если так то эту job нужно удалить
    public sealed class ReplicateEntitiesToCRMJob : TaskServiceJobBase
    {
        private int _chunkSize = 200;
        private int _timeout = 55;
        private readonly IPublicService _publicService;

        public ReplicateEntitiesToCRMJob(
            ICommonLog logger,
            IPublicService publicService,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
            _publicService = publicService;
        }

        public int ChunkSize
        {
            get { return _chunkSize; }
            set { _chunkSize = value; }
        }

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new ReplicateEntitiesToCrmRequest { ChunkSize = ChunkSize, Timeout = Timeout });
        }
    }
}

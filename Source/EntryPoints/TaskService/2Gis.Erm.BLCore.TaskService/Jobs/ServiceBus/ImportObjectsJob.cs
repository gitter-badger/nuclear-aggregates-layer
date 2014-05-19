﻿using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.ServiceBus
{
    [DisallowConcurrentExecution]
    public sealed class ImportObjectsJob : TaskServiceJobBase
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IImportFromServiceBusService _importFromServiceBusService;

        public ImportObjectsJob(ISignInService signInService,
                                IUserImpersonationService userImpersonationService,
                                ICommonLog logger,
                                IIntegrationSettings integrationSettings,
                                IImportFromServiceBusService importFromServiceBusService) : base(signInService, userImpersonationService, logger)
        {
            _integrationSettings = integrationSettings;
            _importFromServiceBusService = importFromServiceBusService;
        }

        public string Flows { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                return;
            }

            var flows = Flows.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var flow in flows)
            {
                _importFromServiceBusService.Import(flow);
            }
        }
    }
}

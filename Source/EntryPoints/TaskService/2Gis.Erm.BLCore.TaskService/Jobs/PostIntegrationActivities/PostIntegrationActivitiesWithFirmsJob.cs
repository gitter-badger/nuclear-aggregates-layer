using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.PostIntegrationActivities
{
    [DisallowConcurrentExecution]
    public sealed class PostIntegrationActivitiesWithFirmsJob : TaskServiceJobBase
    {
        private const int BatchSize = 10;

        private readonly IOperationServicesManager _servicesManager;
        private readonly IGlobalizationSettings _globalizationSettings;

        public PostIntegrationActivitiesWithFirmsJob(ICommonLog logger,
                                                     ISignInService signInService,
                                                     IUserImpersonationService userImpersonationService,
                                                     IOperationServicesManager servicesManager,
                                                     IGlobalizationSettings globalizationSettings)
            : base(signInService, userImpersonationService, logger)
        {
            _servicesManager = servicesManager;
            _globalizationSettings = globalizationSettings;
        }

        public string Activities { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            var flowDescription = new FlowDescription
                {
                    EntityName = EntityName.FirmAddress,
                    IntegrationEntityName = EntityName.ImportedFirmAddress
                };
            SetFlowDescriptionContext(flowDescription);
            var operationsExportService = _servicesManager.GetOperationsExportService(flowDescription.EntityName, flowDescription.IntegrationEntityName);
            while (true)
            {
                var pendingOperations = operationsExportService.GetPendingOperations(BatchSize);
                if (!pendingOperations.Any())
                {
                    break;
                }

                operationsExportService.ExportOperations(flowDescription, pendingOperations, BatchSize);
            }
        }

        private void SetFlowDescriptionContext(FlowDescription description)
        {
            if (description.IntegrationEntityName == EntityName.ImportedFirmAddress)
            {
                var context = new XElement("Localization");
                context.Add(new XAttribute("Language", _globalizationSettings.BasicLanguage));
                description.Context = context.ToString();
            }
        }
    }
}

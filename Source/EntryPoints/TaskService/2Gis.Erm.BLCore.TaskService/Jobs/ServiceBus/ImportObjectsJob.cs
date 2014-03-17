using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
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
        private readonly IPublicService _publicService;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IImportFlowCardsForErmOperationService _importFlowCardsForErmOperationService;

        public ImportObjectsJob(
            IIntegrationSettings integrationSettings, 
            ICommonLog logger, 
            IPublicService publicService, 
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService,
            IImportFlowCardsForErmOperationService importFlowCardsForErmOperationService,
            IIntegrationLocalizationSettings integrationLocalizationSettings)
            : base(signInService, userImpersonationService, logger)
        {
            _integrationSettings = integrationSettings;
            _publicService = publicService;
            _integrationLocalizationSettings = integrationLocalizationSettings;
            _importFlowCardsForErmOperationService = importFlowCardsForErmOperationService;
        }

        public string Flows { get; set; }
        public int PregeneratedIdsAmount { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                return;
            }

            var flows = Flows.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var flow in flows)
            {
                switch (flow.ToLowerInvariant())
                {
                    case "flowphonezones":
                        _publicService.Handle(new ImportFlowPhoneZonesRequest());
                        break;

                    case "flowrubrics":
                        _publicService.Handle(new ImportFlowRubricsRequest
                            {
                                BasicLanguage = _integrationLocalizationSettings.BasicLanguage,
                                ReserveLanguage = _integrationLocalizationSettings.ReserveLanguage
                            });
                        break;

                    case "flowgeography":
                        _publicService.Handle(new ImportFlowGeographyRequest
                            {
                                RegionalTerritoryLocaleSpecificWord = _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord,
                            });
                        break;

                    case "flowgeoclassifier":
                        _publicService.Handle(new ImportFlowGeoClassifierRequest
                            {
                                BasicLanguage = _integrationLocalizationSettings.BasicLanguage,
                                ReserveLanguage = _integrationLocalizationSettings.ReserveLanguage
                            });
                        break;

                    case "flowcards":
                        _publicService.Handle(new ImportFlowCardsRequest
                            {
                                BasicLanguage = _integrationLocalizationSettings.BasicLanguage,
                                ReserveLanguage = _integrationLocalizationSettings.ReserveLanguage,
                                RegionalTerritoryLocaleSpecificWord = _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord,
                                PregeneratedIdsAmount = PregeneratedIdsAmount
                            });
                        break;

                    case "flowstickers":
                        _publicService.Handle(new FlowStickersRequest());
                        break;

                    case "flowfinancialdata1c":
                        _publicService.Handle(new ImportFlowFinancialData1CRequest());
                        break;

                    case "flowcardsforerm":
                        _importFlowCardsForErmOperationService.Import();
                        break;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.ServiceBus
{
    [DisallowConcurrentExecution]
    public sealed class ExportObjectsJob : TaskServiceJobBase
    {
        // Сопоставление ключей, которые могут быть переданы задаче и данных, подлежащих обработке.
        private static readonly IDictionary<string, FlowDescription> KnownFlows = new Dictionary<string, FlowDescription>
            {
                {
                    "floworders.advmaterial",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.Advertisement(),
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_AdvMaterial",
                            IntegrationEntityName = EntityType.Instance.ExportFlowOrdersAdvMaterial()
                        }
                },
                {
                    "floworders.order",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.Order(),
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Order",
                            IntegrationEntityName = EntityType.Instance.ExportFlowOrdersOrder()
                        }
                },
                {
                    "floworders.denialreason",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.DenialReason(),
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_DenialReason",
                            IntegrationEntityName = EntityType.Instance.ExportFlowOrdersDenialReason()
                        }
                },
                {
                    "flowfinancialdata.legalentity",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.LegalPerson(),
                            FlowName = "flowFinancialData",
                            SchemaResourceName = "flowFinancialData_LegalEntity",
                            IntegrationEntityName = EntityType.Instance.ExportFlowFinancialDataLegalEntity()
                        }
                },
                {
                    "floworders.theme",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.Theme(),
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Theme",
                            IntegrationEntityName = EntityType.Instance.ExportFlowOrdersTheme()
                        }
                },
                {
                    "floworders.resource",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.ThemeTemplate(),
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Resource",
                            IntegrationEntityName = EntityType.Instance.ExportFlowOrdersResource()
                        }
                },
                {
                    "floworders.themebranch",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.ThemeOrganizationUnit(),
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_ThemeBranch",
                            IntegrationEntityName = EntityType.Instance.ExportFlowOrdersThemeBranch()
                        }
                },
                {
                    "flowfinancialdata.client",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.Client(),
                            FlowName = "flowFinancialData",
                            SchemaResourceName = "flowFinancialData_Client",
                            IntegrationEntityName = EntityType.Instance.ExportFlowFinancialDataClient()
                        }
                },
                {
                    "flowfinancialdata.debitsinfoinitial",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.AccountDetail(),
                            FlowName = "flowFinancialData",
                            SchemaResourceName = "flowFinancialData_DebitsInfoInitial",
                            IntegrationEntityName = EntityType.Instance.ExportFlowFinancialDataDebitsInfoInitial()
                        }
                },
                {
                    "flowpricelists.pricelist",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.Price(),
                            FlowName = "flowPriceLists",
                            SchemaResourceName = "flowPriceLists_PriceList",
                            IntegrationEntityName = EntityType.Instance.ExportFlowPriceListsPriceList()
                        }
                },
                {
                    "flowpricelists.pricelistposition",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.PricePosition(),
                            FlowName = "flowPriceLists",
                            SchemaResourceName = "flowPriceLists_PriceListPosition",
                            IntegrationEntityName = EntityType.Instance.ExportFlowPriceListsPriceListPosition()
                        }
                },
                {
                    "floworders.invoice",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.Order(),
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Invoice",
                            IntegrationEntityName = EntityType.Instance.ExportFlowOrdersInvoice()
                        }
                },
                {
                    "flownomenclatures.nomenclatureelement",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.Position(),
                            FlowName = "flowNomenclatures",
                            SchemaResourceName = "flowNomenclatures_NomenclatureElement",
                            IntegrationEntityName = EntityType.Instance.ExportFlowNomenclaturesNomenclatureElement()
                        }
                },
                {
                    "flownomenclatures.nomenclatureelementrelation",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.PositionChildren(),
                            FlowName = "flowNomenclatures",
                            SchemaResourceName = "flowNomenclatures_NomenclatureElementRelation",
                            IntegrationEntityName = EntityType.Instance.ExportFlowNomenclaturesNomenclatureElementRelation()
                        }
                },
                {
                    "flowdeliverydata.lettersendrequest",
                    new FlowDescription
                        {
                            EntityName = EntityType.Instance.BirthdayCongratulation(),
                            FlowName = "flowDeliveryData",
                            SchemaResourceName = "flowDeliveryData_LetterSendRequest",
                            IntegrationEntityName = EntityType.Instance.ExportFlowDeliveryDataLetterSendRequest()
                        }
                },
            };

        private readonly IOperationServicesManager _servicesManager;

        public ExportObjectsJob(ITracer tracer,
                                ISignInService signInService,
                                IUserImpersonationService userImpersonationService,
                                IOperationServicesManager servicesManager)
            : base(signInService, userImpersonationService, tracer)
        {
            _servicesManager = servicesManager;
        }

        public string Flows { get; set; }
        public bool ExportInvalidObjects { get; set; }
        public int OperationCount { get; set; }
        public int EntityCount { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (string.IsNullOrEmpty(Flows))
            {
                throw new ArgumentException("Не задан обязательный параметр Flows");
            }

            if (!ExportInvalidObjects && OperationCount <= 0)
            {
                throw new ArgumentException("Не задан обязательный при ExportInvalidObjects=false параметр OperationCount");
            }

            if (EntityCount <= 0)
            {
                throw new ArgumentException("Не задан обязательный параметр EntityCount");
            }

            var flows = Flows.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var flow in flows)
            {
                var description = GetFlowDescription(flow);
                var operationsExportService = _servicesManager.GetOperationsExportService(description.EntityName, description.IntegrationEntityName);

                if (ExportInvalidObjects)
                {
                    operationsExportService.ExportFailedEntities(description, EntityCount);
                }
                else
                {
                    var operations = operationsExportService.GetPendingOperations(OperationCount);
                    operationsExportService.ExportOperations(description, operations, EntityCount);
                }
            }
        }

        private static FlowDescription GetFlowDescription(string flow)
        {
            FlowDescription info;
            if (KnownFlows.TryGetValue(flow.ToLower(), out info))
            {
                return info;
            }

            throw new ArgumentException(string.Format("Поток экспорта {0} системе ERM не известен", flow));
        }
    }
}

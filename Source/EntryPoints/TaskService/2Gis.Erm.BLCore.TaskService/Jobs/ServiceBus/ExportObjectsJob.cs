﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.TaskService.Jobs;

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
                            EntityName = EntityName.Advertisement,
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_AdvMaterial",
                            IntegrationEntityName = EntityName.ExportFlowOrdersAdvMaterial
                        }
                },
                {
                    "floworders.order",
                    new FlowDescription
                        {
                            EntityName = EntityName.Order,
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Order",
                            IntegrationEntityName = EntityName.ExportFlowOrdersOrder
                        }
                },
                {
                    "flowfinancialdata.legalentity",
                    new FlowDescription
                        {
                            EntityName = EntityName.LegalPerson,
                            FlowName = "flowFinancialData",
                            SchemaResourceName = "flowFinancialData_LegalEntity",
                            IntegrationEntityName = EntityName.ExportFlowFinancialDataLegalEntity
                        }
                },
                {
                    "floworders.theme",
                    new FlowDescription
                        {
                            EntityName = EntityName.Theme,
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Theme",
                            IntegrationEntityName = EntityName.ExportFlowOrdersTheme
                        }
                },
                {
                    "floworders.resource",
                    new FlowDescription
                        {
                            EntityName = EntityName.ThemeTemplate,
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Resource",
                            IntegrationEntityName = EntityName.ExportFlowOrdersResource
                        }
                },
                {
                    "floworders.themebranch",
                    new FlowDescription
                        {
                            EntityName = EntityName.ThemeOrganizationUnit,
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_ThemeBranch",
                            IntegrationEntityName = EntityName.ExportFlowOrdersThemeBranch
                        }
                },
                {
                    "flowcardextensions.cardcommercial",
                    new FlowDescription
                        {
                            EntityName = EntityName.FirmAddress,
                            FlowName = "flowCardExtensions",
                            SchemaResourceName = "flowCardExtensions_CardCommercial",
                            IntegrationEntityName = EntityName.ExportFlowCardExtensionsCardCommercial
                        }
                },
                {
                    "flowfinancialdata.client",
                    new FlowDescription
                        {
                            EntityName = EntityName.Client,
                            FlowName = "flowFinancialData",
                            SchemaResourceName = "flowFinancialData_Client",
                            IntegrationEntityName = EntityName.ExportFlowFinancialDataClient
                        }
                },
                {
                    "mscrm.hotclient",
                    new FlowDescription
                        {
                            EntityName = EntityName.HotClientRequest,
                            IntegrationEntityName = EntityName.ExportToMsCrmHotClients
                        }
                },
                {
                    "flowpricelists.pricelist",
                    new FlowDescription
                        {
                            EntityName = EntityName.Price,
                            FlowName = "flowPriceLists",
                            SchemaResourceName = "flowPriceLists_PriceList",
                            IntegrationEntityName = EntityName.ExportFlowPriceListsPriceList
                        }
                },
                {
                    "flowpricelists.pricelistposition",
                    new FlowDescription
                        {
                            EntityName = EntityName.PricePosition,
                            FlowName = "flowPriceLists",
                            SchemaResourceName = "flowPriceLists_PriceListPosition",
                            IntegrationEntityName = EntityName.ExportFlowPriceListsPriceListPosition
                        }
                },
                {
                    "floworders.invoice",
                    new FlowDescription
                        {
                            EntityName = EntityName.Order,
                            FlowName = "flowOrders",
                            SchemaResourceName = "flowOrders_Invoice",
                            IntegrationEntityName = EntityName.ExportFlowOrdersInvoice
                        }
                },
                {
                    "flownomenclatures.nomenclatureelement",
                    new FlowDescription
                        {
                            EntityName = EntityName.Position,
                            FlowName = "flowNomenclatures",
                            SchemaResourceName = "flowNomenclatures_NomenclatureElement",
                            IntegrationEntityName = EntityName.ExportFlowNomenclaturesNomenclatureElement
                        }
                },
                {
                    "flownomenclatures.nomenclatureelementrelation",
                    new FlowDescription
                        {
                            EntityName = EntityName.PositionChildren,
                            FlowName = "flowNomenclatures",
                            SchemaResourceName = "flowNomenclatures_NomenclatureElementRelation",
                            IntegrationEntityName = EntityName.ExportFlowNomenclaturesNomenclatureElementRelation
                        }
                },
            };

        private readonly IOperationServicesManager _servicesManager;
        

        public ExportObjectsJob(ICommonLog logger,
                                ISignInService signInService,
                                IUserImpersonationService userImpersonationService,
                                IOperationServicesManager servicesManager)
            : base(signInService, userImpersonationService, logger)
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

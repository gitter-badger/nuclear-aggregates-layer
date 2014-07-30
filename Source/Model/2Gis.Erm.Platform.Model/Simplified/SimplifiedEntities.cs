﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Simplified
{
    public static class SimplifiedEntities
    {
        public static readonly EntityName[] Entities =
            {
                EntityName.AdditionalFirmService,
                EntityName.Category,
                EntityName.CategoryGroup,
                EntityName.CategoryOrganizationUnit,
                EntityName.RegionalAdvertisingSharing,
                EntityName.NotificationProcessing,
                EntityName.NotificationEmail,
                EntityName.NotificationAddress,
                EntityName.NotificationEmailCc,
                EntityName.NotificationEmailTo,
                EntityName.NotificationEmailAttachment,
                EntityName.ActionsHistory,
                EntityName.ActionsHistoryDetail,
                EntityName.PrintFormTemplate,
                EntityName.Platform,
                EntityName.Project,
                EntityName.Operation,
                EntityName.File,
                EntityName.Note,
                EntityName.BargainType,
                EntityName.ContributionType,
                EntityName.Country,
                EntityName.Currency,
                EntityName.CurrencyRate,
                EntityName.PerformedBusinessOperation,
                EntityName.ExportFlowCardExtensionsCardCommercial,
                EntityName.ExportFlowFinancialDataLegalEntity,
                EntityName.ExportFlowOrdersAdvMaterial,
                EntityName.ExportFlowOrdersOrder,
                EntityName.ExportFlowOrdersInvoice,
                EntityName.ExportFlowOrdersResource,
                EntityName.ExportFlowOrdersTheme,
                EntityName.ExportFlowOrdersThemeBranch,
                EntityName.ExportFlowFinancialDataClient,
                EntityName.ExportFlowPriceListsPriceList,
                EntityName.ExportFlowPriceListsPriceListPosition,
                EntityName.ExportFlowNomenclaturesNomenclatureElement,
                EntityName.ExportFlowNomenclaturesNomenclatureElementRelation,
                EntityName.ImportedFirmAddress,
                EntityName.ExportFailedEntity,
                EntityName.UserEntity,
                EntityName.FileWithContent,
                EntityName.OrderProcessingRequest,
                EntityName.OrderProcessingRequestMessage,
                EntityName.DictionaryEntityInstance,
                EntityName.DictionaryEntityPropertyInstance,
                EntityName.Bank,
                EntityName.Commune,
                EntityName.AcceptanceReportsJournalRecord,
                EntityName.DenialReason,
                EntityName.PerformedOperationPrimaryProcessing,
                EntityName.PerformedOperationFinalProcessing,
                EntityName.BirthdayCongratulation
            };

        private static readonly IDictionary<EntityName, Type> SimplifiedEntitiesMap = new Dictionary<EntityName, Type>();

        static SimplifiedEntities()
        {
            foreach (var simplifiedModelEntity in Entities)
            {
                if (simplifiedModelEntity.IsVirtual())
                {
                    throw new InvalidOperationException("Simplified model entity can't be virtual, because purpose of the simplified model is reuse ERM engine and persistance");
                }

                Type entityType;
                if (!simplifiedModelEntity.TryGetEntityType(out entityType))
                {
                    throw new InvalidOperationException(string.Format("Can't get entity type for simplified model entity {0}, simplified model entity must have entity type for persistance, because purpose of the simplified model is reuse ERM engine and persistance", simplifiedModelEntity));
                }

                SimplifiedEntitiesMap.Add(simplifiedModelEntity, entityType);
            }
        }

        public static bool IsSimplifiedModel(this EntityName entityName)
        {
            return SimplifiedEntitiesMap.ContainsKey(entityName);
        }

        public static bool IsSimplifiedModel(this Type entityType)
        {
            EntityName entityName;
            if (!entityType.TryGetEntityName(out entityName))
            {
                return false;
            }

            return entityName.IsSimplifiedModel();
        }
    }
}

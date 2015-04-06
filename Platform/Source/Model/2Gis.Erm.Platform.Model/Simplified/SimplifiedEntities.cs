using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Simplified
{
    public static class SimplifiedEntities
    {
        // FIXME {all, 23.09.2014}: в списке simplified model смешаны несколько подтипов сущностей ERM, справочные (валюты и т.п.), чисто системые (primaryprocessing, ordervalidationresults и т.п.), необходимо более четко структурировать DomainModel, и все такие сущности явно класифицировать, системная справочная, или, все таки часть какого-то агрегата
        public static readonly EntityName[] Entities =
            {
                EntityName.Category,
                EntityName.CategoryGroup,
                EntityName.CategoryOrganizationUnit,
                EntityName.SalesModelCategoryRestriction,
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
                EntityName.ExportFlowFinancialDataLegalEntity,
                EntityName.ExportFlowOrdersAdvMaterial,
                EntityName.ExportFlowOrdersOrder,
                EntityName.ExportFlowOrdersInvoice,
                EntityName.ExportFlowOrdersResource,
                EntityName.ExportFlowOrdersTheme,
                EntityName.ExportFlowOrdersThemeBranch,
                EntityName.ExportFlowFinancialDataClient,
                EntityName.ExportFlowFinancialDataDebitsInfoInitial,
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
                EntityName.BirthdayCongratulation,
                EntityName.OrderValidationResult,
                EntityName.OrderValidationCacheEntry
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

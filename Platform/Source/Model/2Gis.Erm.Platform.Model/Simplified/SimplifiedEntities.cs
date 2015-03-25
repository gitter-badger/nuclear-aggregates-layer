using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Simplified
{
    public static class SimplifiedEntities
    {
        // FIXME {all, 23.09.2014}: в списке simplified model смешаны несколько подтипов сущностей ERM, справочные (валюты и т.п.), чисто системые (primaryprocessing, ordervalidationresults и т.п.), необходимо более четко структурировать DomainModel, и все такие сущности явно класифицировать, системная справочная, или, все таки часть какого-то агрегата
        public static readonly IEntityType[] Entities =
            {
                EntityType.Instance.AdditionalFirmService(),
                EntityType.Instance.Category(),
                EntityType.Instance.CategoryGroup(),
                EntityType.Instance.CategoryOrganizationUnit(),
                EntityType.Instance.SalesModelCategoryRestriction(),
                EntityType.Instance.NotificationProcessing(),
                EntityType.Instance.NotificationEmail(),
                EntityType.Instance.NotificationAddress(),
                EntityType.Instance.NotificationEmailCc(),
                EntityType.Instance.NotificationEmailTo(),
                EntityType.Instance.NotificationEmailAttachment(),
                EntityType.Instance.ActionsHistory(),
                EntityType.Instance.ActionsHistoryDetail(),
                EntityType.Instance.PrintFormTemplate(),
                EntityType.Instance.Platform(),
                EntityType.Instance.Project(),
                EntityType.Instance.Operation(),
                EntityType.Instance.File(),
                EntityType.Instance.Note(),
                EntityType.Instance.BargainType(),
                EntityType.Instance.ContributionType(),
                EntityType.Instance.Country(),
                EntityType.Instance.Currency(),
                EntityType.Instance.CurrencyRate(),
                EntityType.Instance.PerformedBusinessOperation(),
                EntityType.Instance.ExportFlowCardExtensionsCardCommercial(),
                EntityType.Instance.ExportFlowFinancialDataLegalEntity(),
                EntityType.Instance.ExportFlowOrdersAdvMaterial(),
                EntityType.Instance.ExportFlowOrdersOrder(),
                EntityType.Instance.ExportFlowOrdersInvoice(),
                EntityType.Instance.ExportFlowOrdersResource(),
                EntityType.Instance.ExportFlowOrdersTheme(),
                EntityType.Instance.ExportFlowOrdersThemeBranch(),
                EntityType.Instance.ExportFlowFinancialDataClient(),
                EntityType.Instance.ExportFlowFinancialDataDebitsInfoInitial(),
                EntityType.Instance.ExportFlowPriceListsPriceList(),
                EntityType.Instance.ExportFlowPriceListsPriceListPosition(),
                EntityType.Instance.ExportFlowNomenclaturesNomenclatureElement(),
                EntityType.Instance.ExportFlowNomenclaturesNomenclatureElementRelation(),
                EntityType.Instance.ImportedFirmAddress(),
                EntityType.Instance.ExportFailedEntity(),
                EntityType.Instance.UserEntity(),
                EntityType.Instance.FileWithContent(),
                EntityType.Instance.OrderProcessingRequest(),
                EntityType.Instance.OrderProcessingRequestMessage(),
                EntityType.Instance.DictionaryEntityInstance(),
                EntityType.Instance.DictionaryEntityPropertyInstance(),
                EntityType.Instance.Bank(),
                EntityType.Instance.Commune(),
                EntityType.Instance.AcceptanceReportsJournalRecord(),
                EntityType.Instance.DenialReason(),
                EntityType.Instance.PerformedOperationPrimaryProcessing(),
                EntityType.Instance.PerformedOperationFinalProcessing(),
                EntityType.Instance.BirthdayCongratulation(),
                EntityType.Instance.OrderValidationResult(),
                EntityType.Instance.OrderValidationCacheEntry()
            };

        private static readonly IDictionary<IEntityType, Type> SimplifiedEntitiesMap = new Dictionary<IEntityType, Type>();

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

        public static bool IsSimplifiedModel(this IEntityType entityName)
        {
            return SimplifiedEntitiesMap.ContainsKey(entityName);
        }

        public static bool IsSimplifiedModel(this Type entityType)
        {
            IEntityType entityName;
            if (!entityType.TryGetEntityName(out entityName))
            {
                return false;
            }

            return entityName.IsSimplifiedModel();
        }
    }
}

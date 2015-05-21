using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// В массовой проверке заказов перед сборкой проверять фирмы с рубрикой "Выгодные покупки с 2ГИС" Если по ним нет продаж в текущий месяц позиций привязанных к категориям позиции:
    /// Самореклама только для ПК; 
    /// Выгодные покупки с 2ГИС.
    /// </summary>
    public sealed class AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        public const int AdvantageousPurchasesPositionCategoryExportCode = 25; // Выгодные покупки с 2ГИС.
        public const int AdvantageousPurchasesCategoryId = 18599; // Выгодные покупки с 2ГИС.
        public const int SelfPromotionForThePcPositionCategoryExportCode = 51; // Самореклама только для ПК;
        public const int PcPlatform = 1;
        public const int MobilePlatform = 2;

        private readonly IQuery _query;

        public AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var currentFilter = ruleContext.OrdersFilterPredicate;
            long organizationUnitId;
            long? firmIdForSingleValidation = null;

            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                currentFilter = GetFilterPredicateToGetLinkedOrders(_query, ruleContext.ValidationParams.Single.OrderId, out organizationUnitId, out firmIdForSingleValidation);
            }
            else
            {
                organizationUnitId = ruleContext.ValidationParams.Mass.OrganizationUnitId;
            }

            var advantageousPurchasesFirmIds =
                _query.For<Order>()
                      .Where(currentFilter)
                      .SelectMany(x => x.OrderPositions)
                      .Where(y => y.IsActive && !y.IsDeleted)
                      .SelectMany(x => x.OrderPositionAdvertisements
                                        .Select(y => y.Position)
                                        .Where(y =>
                                               y.PositionCategory.ExportCode == AdvantageousPurchasesPositionCategoryExportCode
                                               || y.PositionCategory.ExportCode == SelfPromotionForThePcPositionCategoryExportCode)
                                        .Select(y => new
                                            {
                                                PlatformDgppId = y.Platform.DgppId,
                                                PositionCategoryExportCode = y.PositionCategory.ExportCode,
                                                FirmId = x.Order.FirmId
                                            }))
                      .ToArray()
                      .GroupBy(x => x.FirmId)
                      .Where(x => x.Any(y => y.PositionCategoryExportCode == SelfPromotionForThePcPositionCategoryExportCode)
                                  || x.Any(y => y.PositionCategoryExportCode == AdvantageousPurchasesPositionCategoryExportCode && y.PlatformDgppId == PcPlatform))
                      .Select(x => x.Key)
                      .ToArray();

            var firmsWithoutPurchasesErrors =
                _query.For<Firm>()
                      .Where(firm => firm.OrganizationUnitId == organizationUnitId &&
                                           firm.IsActive &&
                                           !firm.IsDeleted &&
                                           !firm.ClosedForAscertainment &&
                                           firm.FirmAddresses.Any(firmAddress => firmAddress.IsActive && !firmAddress.IsDeleted &&
                                                                                 firmAddress.CategoryFirmAddresses
                                                                                            .Any(addressCategory => addressCategory.IsActive &&
                                                                                                                    !addressCategory.IsDeleted &&
                                                                                                                    addressCategory.Category.Id ==
                                                                                                                    AdvantageousPurchasesCategoryId)) &&
                                           !advantageousPurchasesFirmIds.Contains(firm.Id) &&
                                           (ruleContext.ValidationParams.IsMassValidation || (firmIdForSingleValidation.HasValue && firm.Id == firmIdForSingleValidation.Value)))
                      .Select(x => new { x.Id, x.Name })
                      .AsEnumerable()
                      .Select(x => new OrderValidationMessage
                      {
                          Type = ruleContext.ValidationParams.IsMassValidation ? MessageType.Error : MessageType.Warning,
                          MessageText = string.Format(BLResources.ThereIsNoAdvertisementForAdvantageousPurchasesCategory, GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.Firm(), x.Name, x.Id))
                      });

            return firmsWithoutPurchasesErrors;
        }
    }
}

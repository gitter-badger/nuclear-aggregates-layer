using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// В массовой проверке заказов перед сборкой проверять фирмы с рубрикой "Выгодные покупки с 2ГИС" Если по ним нет продаж в текущий месяц позиций привязанных к категориям позиции:
    /// Самореклама только для ПК; 
    /// Выгодные покупки с 2ГИС.
    /// </summary>
    public sealed class AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        public const int AdvantageousPurchasesPositionCategoryExportCode = 25; // Выгодные покупки с 2ГИС.
        public const int AdvantageousPurchasesCategoryId = 18599; // Выгодные покупки с 2ГИС.
        public const int SelfPromotionForThePcPositionCategoryExportCode = 51; // Самореклама только для ПК;
        public const int PcPlatform = 1;
        public const int MobilePlatform = 2;

        private readonly IFinder _finder;

        public AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var currentFilter = filterPredicate;
            long organizationUnitId;
            long? firmIdForSingleValidation = null;

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (!IsCheckMassive)
            {
                if (request.OrderId == null)
                {
                    throw new ArgumentNullException("OrderId");
                }

                currentFilter = GetFilterPredicateToGetLinkedOrders(_finder, request.OrderId.Value, out organizationUnitId, out firmIdForSingleValidation);
            }
            else
            {
                if (request.OrganizationUnitId == null)
                {
                    throw new ArgumentNullException("OrganizationUnitId");
                }

                organizationUnitId = request.OrganizationUnitId.Value;
            }

            var advantageousPurchasesFirmIds =
                _finder.Find(currentFilter)
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

            var firmsWithoutPurchases =
                _finder.Find<Firm>(firm => firm.OrganizationUnitId == organizationUnitId &&
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
                                           (IsCheckMassive || (firmIdForSingleValidation.HasValue && firm.Id == firmIdForSingleValidation.Value)))
                      .Select(x => new { x.Id, x.Name })
                      .ToArray();

            foreach (var firm in firmsWithoutPurchases)
            {
                var firmDescription = GenerateDescription(EntityName.Firm, firm.Name, firm.Id);
                messages.Add(new OrderValidationMessage
                    {
                        Type = IsCheckMassive ? MessageType.Error : MessageType.Warning,
                        MessageText = string.Format(BLResources.ThereIsNoAdvertisementForAdvantageousPurchasesCategory, firmDescription)
                    });
            }
        }
    }
}

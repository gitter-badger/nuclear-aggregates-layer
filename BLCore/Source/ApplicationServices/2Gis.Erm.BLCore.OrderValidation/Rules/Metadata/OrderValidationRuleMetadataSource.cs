using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Metadata
{
    public sealed class OrderValidationRuleMetadataSource : MetadataSourceBase<MetadataOrderValidationIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        #region RulesMetadata

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        private readonly OrderValidationRuleGroupMetadata _genericGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                                            .Group(OrderValidationRuleGroup.Generic)
                                            .Rules(OrderValidationRuleMetadata.Config.Rule<BargainOutOfDateOrderValidationRule>(1)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<CouponPeriodOrderValidationRule>(2)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<AccountExistsOrderValidationRule>(3)
                                                                              .AvailableFor(ValidationType.PreReleaseBeta)
                                                                              .AvailableFor(ValidationType.PreReleaseFinal),
                                                   OrderValidationRuleMetadata.Config.Rule<BillsSumsOrderValidationRule>(7)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<CategoriesLinkedToDestOrgUnitOrderValidationRule>(8)
                                                                              .NonManual,
                                                   OrderValidationRuleMetadata.Config.Rule<DistributionDatesOrderValidationRule>(9)
                                                                              .NonManual,
                                                   OrderValidationRuleMetadata.Config.Rule<FirmBelongsToOrdersDestOrgUnitOrderValidationRule>(10)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<FirmsOrderValidationRule>(11)
                                                                              .NonManual,
                                                   OrderValidationRuleMetadata.Config.Rule<LinkingObjectsOrderValidationRule>(12)
                                                                              .NonManual,
                                                   OrderValidationRuleMetadata.Config.Rule<LockNoExistsOrderValidationRule>(13)
                                                                              .AvailableFor(ValidationType.PreReleaseBeta)
                                                                              .AvailableFor(ValidationType.PreReleaseFinal),
                                                   OrderValidationRuleMetadata.Config.Rule<OrderHasAtLeastOnePositionOrderValidationRule>(14)
                                                                              .NonManual,
                                                   OrderValidationRuleMetadata.Config.Rule<OrderPositionsRefereceCurrentPriceListOrderValidationRule>(15)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<OrdersAndBargainsScansExistOrderValidationRule>(16)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<ReleaseNotExistsOrderValidationRule>(17)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<RequiredFieldsNotEmptyOrderValidationRule>(18)
                                                                              .NonManual,
                                                   OrderValidationRuleMetadata.Config.Rule<BalanceOrderValidationRule>(20)
                                                                              .AvailableFor(ValidationType.PreReleaseFinal)
                                                                              .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                                                   OrderValidationRuleMetadata.Config.Rule<LegalPersonProfilesOrderValidationRule>(23)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<WarrantyEndDateOrderValidationRule>(24)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<BargainEndDateOrderValidationRule>(25)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<ContactDoesntContainSponsoredLinkOrderValidationRule>(27)
                                                                              .SingleOrderValidation,
                                                   OrderValidationRuleMetadata.Config.Rule<AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule>(29)
                                                                              .SingleOrderValidation
                                                                              .AvailableFor(ValidationType.PreReleaseBeta)
                                                                              .AvailableFor(ValidationType.PreReleaseFinal)
                                                                              .AvailableFor(ValidationType.ManualReport)
                                                                              .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                                                   OrderValidationRuleMetadata.Config.Rule<CategoriesForFirmAmountOrderValidationRule>(32)
                                                                              .SingleOrderValidation
                                                                              .AvailableFor(ValidationType.PreReleaseBeta)
                                                                              .AvailableFor(ValidationType.ManualReport)
                                                                              .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                                                   OrderValidationRuleMetadata.Config.Rule<IsPositionSupportedByExportOrderValidationRule>(33)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule>(34)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<CouponIsUniqueForFirmOrderValidationRule>(35)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<SelfAdvertisementOrderValidationRule>(36)
                                                                              .Common,

                                                   // Временно отключена ERM-6155
                                                   // OrderValidationRuleMetadata.Config.Rule<AdditionalAdvertisementsOrderValidationRule>(37)
                                                   //                            .Common,
                                                   OrderValidationRuleMetadata.Config
                                                                              .Rule<IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule>(38)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<PlatformTypeOrderValidationRule>(39)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<DefaultThemeMustBeSpecifiedValidationRule>(40)
                                                                              .AvailableFor(ValidationType.PreReleaseBeta)
                                                                              .AvailableFor(ValidationType.PreReleaseFinal)
                                                                              .AvailableFor(ValidationType.ManualReport)
                                                                              .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                                                   OrderValidationRuleMetadata.Config.Rule<DefaultThemeMustContainOnlySelfAdvValidationRule>(41)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<ThemePeriodOverlapsOrderPeriodValidationRule>(42)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<ThemeCategoriesValidationRule>(43)
                                                                              .AvailableFor(ValidationType.PreReleaseBeta)
                                                                              .AvailableFor(ValidationType.PreReleaseFinal)
                                                                              .AvailableFor(ValidationType.ManualReport)
                                                                              .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                                                   OrderValidationRuleMetadata.Config.Rule<ThemePositionCountValidationRule>(44)
                                                                              .Common);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        private readonly OrderValidationRuleGroupMetadata _salesModelGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                                            .Group(OrderValidationRuleGroup.SalesModelValidation)
                                            .UseCaching
                                            .Rules(OrderValidationRuleMetadata.Config.Rule<SalesModelRestrictionsOrderValidationRule>(45).Common);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        private readonly OrderValidationRuleGroupMetadata _advertisementMaterialsGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                                            .Group(OrderValidationRuleGroup.AdvertisementMaterialsValidation)
                                            .UseCaching
                                            .Rules(OrderValidationRuleMetadata.Config.Rule<AdvertisementsOnlyWhiteListOrderValidationRule>(21)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<AdvertisementsWithoutWhiteListOrderValidationRule>(22)
                                                                              .Common,
                                                   OrderValidationRuleMetadata.Config.Rule<DummyAdvertisementOrderValidationRule>(47)
                                                                              .Common);

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        private readonly OrderValidationRuleGroupMetadata _adPositionsGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                                            .Group(OrderValidationRuleGroup.ADPositionsValidation)
                                            .UseCaching
                                            .EnableCachingFor(ValidationType.PreReleaseBeta)
                                            .EnableCachingFor(ValidationType.PreReleaseFinal)
                                            .Rules(OrderValidationRuleMetadata.Config.Rule<AssociatedAndDeniedPricePositionsOrderValidationRule>(6)
                                                                              .Common
                                                                              .AvailableFor(ValidationType.SingleOrderOnStateChanging));

        // нужно чтобы группа проверок, OrderValidationRuleGroup.AdvertisementAmountValidation всегда шла последней, т.к. в ней есть правила (например, AdvertisementAmountOrderValidationRule), анализирующие результаты работы правил из предыдущих групп
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        private readonly OrderValidationRuleGroupMetadata _advertisementAmountGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                                            .Group(OrderValidationRuleGroup.AdvertisementAmountValidation)
                                            .Rules(OrderValidationRuleMetadata.Config.Rule<AdvertisementAmountOrderValidationRule>(26)
                                                                              .NonManual,
                                                   OrderValidationRuleMetadata.Config.Rule<AdvertisementForCategoryAmountOrderValidationRule>(31)
                                                                              .Common);
        #endregion

        public OrderValidationRuleMetadataSource()
        {
            HierarchyMetadata orderValidationRulesMetadataRoot =
                HierarchyMetadata.Config
                    .Id.Is(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataOrderValidationIdentity>("Rules"))
                    .Childs( // автоматически не заполняем, т.к. нарушается сортировка - элементов немного пока заполняем вручную 
                        _genericGroupMetadata,
                        _salesModelGroupMetadata,
                        _advertisementMaterialsGroupMetadata,
                        _adPositionsGroupMetadata,
                        _advertisementAmountGroupMetadata);

            _metadata = new Dictionary<Uri, IMetadataElement> { { orderValidationRulesMetadataRoot.Identity.Id, orderValidationRulesMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}

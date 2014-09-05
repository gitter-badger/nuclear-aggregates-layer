using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Metadata
{
    public sealed class OrderValidationRuleMetadataSource : MetadataSourceBase<MetadataOrderValidationRulesIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        #region RulesMetadata

        private readonly IReadOnlyCollection<OrderValidationRuleMetadata> _originalMetadata = new OrderValidationRuleMetadata[]
            {
                OrderValidationRuleMetadata.Config.Rule<BargainOutOfDateOrderValidationRule>(OrderValidationRuleGroup.Generic, 1)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<CouponPeriodOrderValidationRule>(OrderValidationRuleGroup.Generic, 2)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<AccountExistsOrderValidationRule>(OrderValidationRuleGroup.Generic, 3)
                                                                                .AvailableFor(ValidationType.PreReleaseBeta)
                                                                                .AvailableFor(ValidationType.PreReleaseFinal),
                OrderValidationRuleMetadata.Config.Rule<BillsSumsOrderValidationRule>(OrderValidationRuleGroup.Generic, 7)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<CategoriesLinkedToDestOrgUnitOrderValidationRule>(OrderValidationRuleGroup.Generic, 8)
                                                                                .NonManual,
                OrderValidationRuleMetadata.Config.Rule<DistributionDatesOrderValidationRule>(OrderValidationRuleGroup.Generic, 9)
                                                                                .NonManual,
                OrderValidationRuleMetadata.Config.Rule<FirmBelongsToOrdersDestOrgUnitOrderValidationRule>(OrderValidationRuleGroup.Generic, 10)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<FirmsOrderValidationRule>(OrderValidationRuleGroup.Generic, 11)
                                                                                .NonManual,
                OrderValidationRuleMetadata.Config.Rule<LinkingObjectsOrderValidationRule>(OrderValidationRuleGroup.Generic, 12)
                                                                                .NonManual,
                OrderValidationRuleMetadata.Config.Rule<LockNoExistsOrderValidationRule>(OrderValidationRuleGroup.Generic, 13)
                                                                                .AvailableFor(ValidationType.PreReleaseBeta)
                                                                                .AvailableFor(ValidationType.PreReleaseFinal),
                OrderValidationRuleMetadata.Config.Rule<OrderHasAtLeastOnePositionOrderValidationRule>(OrderValidationRuleGroup.Generic, 14)
                                                                                .NonManual,
                OrderValidationRuleMetadata.Config.Rule<OrderPositionsRefereceCurrentPriceListOrderValidationRule>(OrderValidationRuleGroup.Generic, 15)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<OrdersAndBargainsScansExistOrderValidationRule>(OrderValidationRuleGroup.Generic, 16)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<ReleaseNotExistsOrderValidationRule>(OrderValidationRuleGroup.Generic, 17)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<RequiredFieldsNotEmptyOrderValidationRule>(OrderValidationRuleGroup.Generic, 18)
                                                                                .NonManual,
                OrderValidationRuleMetadata.Config.Rule<BalanceOrderValidationRule>(OrderValidationRuleGroup.Generic, 20)
                                                                                .AvailableFor(ValidationType.PreReleaseFinal)
                                                                                .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                OrderValidationRuleMetadata.Config.Rule<LegalPersonProfilesOrderValidationRule>(OrderValidationRuleGroup.Generic, 23)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<WarrantyEndDateOrderValidationRule>(OrderValidationRuleGroup.Generic, 24)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<BargainEndDateOrderValidationRule>(OrderValidationRuleGroup.Generic, 25)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<ContactDoesntContainSponsoredLinkOrderValidationRule>(OrderValidationRuleGroup.Generic, 27)
                                                                                .SingleOrderValidation,
                OrderValidationRuleMetadata.Config.Rule<AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule>(OrderValidationRuleGroup.Generic, 29)
                                                                                .SingleOrderValidation
                                                                                .AvailableFor(ValidationType.PreReleaseBeta)
                                                                                .AvailableFor(ValidationType.PreReleaseFinal)
                                                                                .AvailableFor(ValidationType.ManualReport)
                                                                                .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                OrderValidationRuleMetadata.Config.Rule<CategoriesForFirmAmountOrderValidationRule>(OrderValidationRuleGroup.Generic, 32)
                                                                                .SingleOrderValidation
                                                                                .AvailableFor(ValidationType.PreReleaseBeta)
                                                                                .AvailableFor(ValidationType.ManualReport)
                                                                                .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                OrderValidationRuleMetadata.Config.Rule<IsPositionSupportedByExportOrderValidationRule>(OrderValidationRuleGroup.Generic, 33)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule>(OrderValidationRuleGroup.Generic, 34)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<CouponIsUniqueForFirmOrderValidationRule>(OrderValidationRuleGroup.Generic, 35)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<SelfAdvertisementOrderValidationRule>(OrderValidationRuleGroup.Generic, 36)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<AdditionalAdvertisementsOrderValidationRule>(OrderValidationRuleGroup.Generic, 37)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule>(OrderValidationRuleGroup.Generic, 38)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<PlatformTypeOrderValidationRule>(OrderValidationRuleGroup.Generic, 39)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<DefaultThemeMustBeSpecifiedValidationRule>(OrderValidationRuleGroup.Generic, 40)
                                                                                .AvailableFor(ValidationType.PreReleaseBeta)
                                                                                .AvailableFor(ValidationType.PreReleaseFinal)
                                                                                .AvailableFor(ValidationType.ManualReport)
                                                                                .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                OrderValidationRuleMetadata.Config.Rule<DefaultThemeMustContainOnlySelfAdvValidationRule>(OrderValidationRuleGroup.Generic, 41)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<ThemePeriodOverlapsOrderPeriodValidationRule>(OrderValidationRuleGroup.Generic, 42)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<ThemeCategoriesValidationRule>(OrderValidationRuleGroup.Generic, 43)
                                                                                .AvailableFor(ValidationType.PreReleaseBeta)
                                                                                .AvailableFor(ValidationType.PreReleaseFinal)
                                                                                .AvailableFor(ValidationType.ManualReport)
                                                                                .AvailableFor(ValidationType.ManualReportWithAccountsCheck),
                OrderValidationRuleMetadata.Config.Rule<AdvertisementsOnlyWhiteListOrderValidationRule>(OrderValidationRuleGroup.AdvertisementMaterialsValidation, 21)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<AdvertisementsWithoutWhiteListOrderValidationRule>(OrderValidationRuleGroup.AdvertisementMaterialsValidation, 22)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<DummyAdvertisementOrderValidationRule>(OrderValidationRuleGroup.AdvertisementMaterialsValidation, 47)
                                                                                .Common,
                OrderValidationRuleMetadata.Config.Rule<AssociatedAndDeniedPricePositionsOrderValidationRule>(OrderValidationRuleGroup.ADPositionsValidation, 6)
                                                                                .Common
                                                                                .AvailableFor(ValidationType.SingleOrderOnStateChanging),
                OrderValidationRuleMetadata.Config.Rule<AdvertisementAmountOrderValidationRule>(OrderValidationRuleGroup.AdvertisementAmountValidation, 26)
                                                                                .NonManual,
                OrderValidationRuleMetadata.Config.Rule<AdvertisementForCategoryAmountOrderValidationRule>(OrderValidationRuleGroup.AdvertisementAmountValidation, 31)
                                                                                .Common
            };

        #endregion

        public OrderValidationRuleMetadataSource()
        {
            _metadata = _originalMetadata.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}

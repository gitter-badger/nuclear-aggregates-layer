using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.OrderValidation.MultiCulture
{
    public sealed class OrderValidationRuleMetadataSource : MetadataSourceBase<MetadataOrderValidationIdentity>, ICyprusAdapted, ICzechAdapted, IChileAdapted, IEmiratesAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        #region RulesMetadata

        private readonly OrderValidationRuleGroupMetadata _genericGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                .Group(OrderValidationRuleGroup.Generic)
                .Rules(OrderValidationRuleMetadata.Config.Rule<AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule>(29)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                        OrderValidationRuleMetadata.Config.Rule<IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule>(34)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                        OrderValidationRuleMetadata.Config.Rule<CouponIsUniqueForFirmOrderValidationRule>(35)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                        OrderValidationRuleMetadata.Config.Rule<SelfAdvertisementOrderValidationRule>(36)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                        OrderValidationRuleMetadata.Config.Rule<AdditionalAdvertisementsOrderValidationRule>(37)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                        OrderValidationRuleMetadata.Config.Rule<IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule>(38)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates));
        private readonly OrderValidationRuleGroupMetadata _advertisementMaterialsGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                .Group(OrderValidationRuleGroup.AdvertisementMaterialsValidation)
                .Rules(OrderValidationRuleMetadata.Config.Rule<AdvertisementsWithoutWhiteListOrderValidationRule>(22)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates));

         private readonly OrderValidationRuleGroupMetadata _advertisementAmountGroupMetadata =
            OrderValidationRuleGroupMetadata.Config
                .Group(OrderValidationRuleGroup.AdvertisementAmountValidation)
                .Rules(OrderValidationRuleMetadata.Config.Rule<AdvertisementForCategoryAmountOrderValidationRule>(31)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates));
        #endregion

        public OrderValidationRuleMetadataSource()
        {
            _metadata = new Dictionary<Uri, IMetadataElement>
                {
                    { _genericGroupMetadata.Identity.Id, _genericGroupMetadata },
                    { _advertisementMaterialsGroupMetadata.Identity.Id, _advertisementMaterialsGroupMetadata },
                    { _advertisementAmountGroupMetadata.Identity.Id, _advertisementAmountGroupMetadata },
                };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}

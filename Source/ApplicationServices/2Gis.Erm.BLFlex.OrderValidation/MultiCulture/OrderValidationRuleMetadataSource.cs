using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.OrderValidation.MultiCulture
{
    public sealed class OrderValidationRuleMetadataSource : MetadataSourceBase<MetadataOrderValidationRulesIdentity>, ICyprusAdapted, ICzechAdapted, IChileAdapted, IEmiratesAdapted
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        #region RulesMetadata

        private readonly IReadOnlyCollection<OrderValidationRuleMetadata> _originalMetadata = new OrderValidationRuleMetadata[]
            {
                OrderValidationRuleMetadata.Config.Rule<AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule>(OrderValidationRuleGroup.Generic, 29)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                OrderValidationRuleMetadata.Config.Rule<IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule>(OrderValidationRuleGroup.Generic, 34)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                OrderValidationRuleMetadata.Config.Rule<CouponIsUniqueForFirmOrderValidationRule>(OrderValidationRuleGroup.Generic, 35)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                OrderValidationRuleMetadata.Config.Rule<SelfAdvertisementOrderValidationRule>(OrderValidationRuleGroup.Generic, 36)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                OrderValidationRuleMetadata.Config.Rule<AdditionalAdvertisementsOrderValidationRule>(OrderValidationRuleGroup.Generic, 37)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                OrderValidationRuleMetadata.Config.Rule<IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule>(OrderValidationRuleGroup.Generic, 38)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                OrderValidationRuleMetadata.Config.Rule<AdvertisementsWithoutWhiteListOrderValidationRule>(OrderValidationRuleGroup.AdvertisementMaterialsValidation, 22)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates),
                OrderValidationRuleMetadata.Config.Rule<AdvertisementForCategoryAmountOrderValidationRule>(OrderValidationRuleGroup.AdvertisementAmountValidation, 31)
                                                                                .DisableFor(BusinessModel.Cyprus)
                                                                                .DisableFor(BusinessModel.Czech)
                                                                                .DisableFor(BusinessModel.Chile)
                                                                                .DisableFor(BusinessModel.Emirates)
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

using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AdsTemplatesAdsElementTemplate =
            CardMetadata.Config
                        .For<AdsTemplatesAdsElementTemplate>()
                        .MainAttribute<AdsTemplatesAdsElementTemplate>(x => x.Id)
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigCommonCardToolbarButtons<AdsTemplatesAdsElementTemplate>());
    }
}
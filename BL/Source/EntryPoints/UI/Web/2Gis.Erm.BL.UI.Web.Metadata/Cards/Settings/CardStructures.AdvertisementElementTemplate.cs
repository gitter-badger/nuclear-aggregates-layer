using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AdvertisementElementTemplate =
            CardMetadata.For<AdvertisementElementTemplate>()
                        .MainAttribute<AdvertisementElementTemplate, IAdvertisementElementTemplateViewModel>(x => x.Name)
                        .ConfigCommonCardToolbar()
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Children")
                                                             .Title.Resource(() => ErmConfigLocalization.EnMAdvertisementTemplate)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.AdsTemplatesAdsElementTemplate)
                                                             .FilterToParent());
    }
}
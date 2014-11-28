using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AdvertisementElementTemplate =
            CardMetadata.Config
                        .For<AdvertisementElementTemplate>()
                        .MainAttribute<AdvertisementElementTemplate, IAdvertisementElementTemplateViewModel>(x => x.Name)
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigCommonCardToolbarButtons<AdvertisementElementTemplate>())
                        .RelatedItems
                            .Name("Information")
                            .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                            .Attach(UiElementMetadata.Config.ContentTab(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Children")
                                                     .Title.Resource(() => ErmConfigLocalization.EnMAdvertisementTemplate)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.AdsTemplatesAdsElementTemplate));
    }
}
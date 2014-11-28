using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AdvertisementTemplate =
            CardMetadata.Config
                        .For<AdvertisementTemplate>()
                        .MainAttribute<AdvertisementTemplate, IAdvertisementTemplateViewModel>(x => x.Name)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<AdvertisementTemplate>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<AdvertisementTemplate>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<AccountDetail>(),
                                    UiElementMetadata.Config
                                                     .Name.Static("PublishAdvertisementTemplate")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlPublishAdvertisementTemplate)
                                                     .ControlType(ControlType.TextImageButton)
                                                     .LockOnInactive()
                                                     .LockOnNew()
                                                     .Handler.Name("scope.Publish")
                                                     .Icon.Path("Refresh.gif"),
                                    UiElementMetadata.Config
                                                     .Name.Static("UnpublishAdvertisementTemplate")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlUnpublishAdvertisementTemplate)
                                                     .ControlType(ControlType.TextImageButton)
                                                     .LockOnNew()
                                                     .Handler.Name("scope.Unpublish")
                                                     .Icon.Path("Refresh.gif"),
                                    UiElementMetadata.Config.CloseAction())
                        .RelatedItems
                            .Name("Information")
                            .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                            .Attach(UiElementMetadata.Config.ContentTab(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Children")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelChildrenPositions)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.AdsTemplatesAdsElementTemplate));
    }
}
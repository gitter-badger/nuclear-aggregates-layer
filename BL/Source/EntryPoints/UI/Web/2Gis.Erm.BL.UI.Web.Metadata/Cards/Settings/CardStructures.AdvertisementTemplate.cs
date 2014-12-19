using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AdvertisementTemplate =
            CardMetadata.For<AdvertisementTemplate>()
                        .MainAttribute<AdvertisementTemplate, IAdvertisementTemplateViewModel>(x => x.Name)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<AdvertisementTemplate>(),
                                UIElementMetadata.Config.UpdateAction<AdvertisementTemplate>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<AdvertisementTemplate>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<AdvertisementTemplate>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<AdvertisementTemplate>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config
                                                 .Name.Static("PublishAdvertisementTemplate")
                                                 .Title.Resource(() => ErmConfigLocalization.ControlPublishAdvertisementTemplate)
                                                 .ControlType(ControlType.TextImageButton)
                                                 .LockOnInactive()
                                                 .LockOnNew()
                                                 .Handler.Name("scope.Publish")
                                                 .Icon.Path("Refresh.gif"),
                                UIElementMetadata.Config
                                                 .Name.Static("UnpublishAdvertisementTemplate")
                                                 .Title.Resource(() => ErmConfigLocalization.ControlUnpublishAdvertisementTemplate)
                                                 .ControlType(ControlType.TextImageButton)
                                                 .LockOnNew()
                                                 .Handler.Name("scope.Unpublish")
                                                 .Icon.Path("Refresh.gif"),
                                UIElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(
                                            UIElementMetadata.Config.ContentTab(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Children")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelChildrenPositions)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.AdsTemplatesAdsElementTemplate)
                                                             .FilterToParent());
    }
}
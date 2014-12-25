using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AdvertisementTemplate =
            CardMetadata.For<AdvertisementTemplate>()
                        .WithDefaultIcon()
                        .Actions
                        .Attach(ToolbarElements.Create<AdvertisementTemplate>(),
                                ToolbarElements.Update<AdvertisementTemplate>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<AdvertisementTemplate>(),
                                ToolbarElements.UpdateAndClose<AdvertisementTemplate>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<AdvertisementTemplate>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.AdvertisementTemplates.Publish()
                                               .DisableOn<IAdvertisementTemplateViewModel>(x => x.IsPublished),
                                ToolbarElements.AdvertisementTemplates.Unpublish()
                                               .DisableOn<IAdvertisementTemplateViewModel>(x => !x.IsPublished),
                                ToolbarElements.Close())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Children")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelChildrenPositions)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.AdsTemplatesAdsElementTemplate)
                                                           .FilterToParent());
    }
}
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AdvertisementTemplate =
            CardMetadata.For<AdvertisementTemplate>()
                        .WithDefaultIcon()
                        .InfoOn<IPublishableAspect>(x => x.IsPublished, StringResourceDescriptor.Create(() => BLResources.CanNotChangePublishedAdvertisementTemplate))
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
                                               .DisableOn<IPublishableAspect>(x => x.IsPublished),
                                ToolbarElements.AdvertisementTemplates.Unpublish()
                                               .DisableOn<IPublishableAspect>(x => !x.IsPublished),
                                ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem.ChildrenGrid(EntityName.AdsTemplatesAdsElementTemplate, () => ErmConfigLocalization.CrdRelChildrenPositions));
    }
}
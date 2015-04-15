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
        public static readonly CardMetadata Price =
            CardMetadata.For<Price>()
                        .InfoOn<IDeletableAspect>(x => x.IsDeleted, StringResourceDescriptor.Create(() => BLResources.CantEditPriceWhenDeactivated))
                        .InfoOn<IPublishableAspect>(x => x.IsPublished, StringResourceDescriptor.Create(() => BLResources.CantEditPriceWhenPublished))
                        .WithEntityIcon()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityName.Price)),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.PricePosition,
                                                                              Icons.Icons.Entity.Small(EntityName.PricePosition),
                                                                              () => ErmConfigLocalization.CrdRelPricePosition))
                        .Actions
                        .Attach(ToolbarElements.Create<Price>(),
                                ToolbarElements.Update<Price>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Price>(),
                                ToolbarElements.UpdateAndClose<Price>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Price>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Prices.Publish()
                                               .DisableOn<IPublishableAspect>(x => x.IsPublished),
                                ToolbarElements.Prices.Unpublish()
                                               .DisableOn<IPublishableAspect>(x => !x.IsPublished),
                                ToolbarElements.Prices.Copy(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close());
    }
}
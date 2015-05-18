using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Price =
            CardMetadata.For<Price>()
                        .InfoOn<IDeletableAspect>(x => x.IsDeleted, StringResourceDescriptor.Create(() => BLResources.CantEditPriceWhenDeactivated))
                        .InfoOn<IPublishableAspect>(x => x.IsPublished, StringResourceDescriptor.Create(() => BLResources.CantEditPriceWhenPublished))
                        .WithEntityIcon()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.Price())),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.PricePosition(),
                                                                              Icons.Icons.Entity.Small(EntityType.Instance.PricePosition()),
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
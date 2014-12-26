using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Price =
            CardMetadata.For<Price>()
                        .Icon.Path(Icons.Icons.Entity.Price)
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.PriceSmall),
                                          RelatedItems.RelatedItem.EntityGrid(EntityName.PricePosition, Icons.Icons.Entity.PricePositionSmall, () => ErmConfigLocalization.CrdRelPricePosition))
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
                                               .DisableOn<IPriceViewModel>(x => x.IsPublished),
                                ToolbarElements.Prices.Unpublish()
                                               .DisableOn<IPriceViewModel>(x => !x.IsPublished),
                                ToolbarElements.Prices.Copy(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close());
    }
}
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
        public static readonly CardMetadata Price =
            CardMetadata.For<Price>()
                        .Icon.Path(Icons.Icons.Entity.Price)
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(Icons.Icons.Entity.PriceSmall),
                                          UIElementMetadata.Config
                                                           .Name.Static("PricePosition")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelPricePosition)
                                                           .Icon.Path(Icons.Icons.Entity.PricePositionSmall)
                                                           .Handler.ShowGridByConvention(EntityName.PricePosition)
                                                           .LockOnNew()
                                                           .FilterToParent())
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
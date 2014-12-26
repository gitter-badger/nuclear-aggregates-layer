using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Position =
            CardMetadata.For<Position>()
                        .Icon.Path(Icons.Icons.Entity.Position)
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.PositionSmall),
                                          RelatedItems.RelatedItem.ChildrenGrid(EntityName.PositionChildren, () => ErmConfigLocalization.CrdRelChildrenPositions)
                                                      .DisableOn<IPositionViewModel>(x => !x.IsComposite));
    }
}
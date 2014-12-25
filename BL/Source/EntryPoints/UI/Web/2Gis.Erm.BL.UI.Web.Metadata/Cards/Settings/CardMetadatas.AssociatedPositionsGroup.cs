using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AssociatedPositionsGroup =
            CardMetadata.For<AssociatedPositionsGroup>()
                        .WithDefaultIcon()
                        .CommonCardToolbar()
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                          UIElementMetadata.Config.Name.Static("AssociatedPosition")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelAssociatedPosition)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.AssociatedPosition)
                                                           .FilterToParent());
    }
}
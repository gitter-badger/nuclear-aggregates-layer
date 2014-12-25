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
        public static readonly CardMetadata Bargain =
            CardMetadata.For<Bargain>()
                        .Icon.Path(Icons.Icons.Entity.Bargain)
                        .WithRelatedItems(
                                          UIElementMetadata.Config.ContentTab(),
                                          UIElementMetadata.Config
                                                           .Name.Static("BargainFiles")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelBargainFiles)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.BargainFile)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Orders")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Order)
                                                           .FilterToParent());
    }
}
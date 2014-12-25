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
        public static readonly CardMetadata LegalPerson =
            CardMetadata.For<LegalPerson>()
                        .WithDefaultIcon()
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Account")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelAccounts)
                                                           .Icon.Path(Icons.Icons.Entity.Account)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Account)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Limits")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Limit)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Bargains")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelBargains)
                                                           .Icon.Path(Icons.Icons.Entity.BargainSmall)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Bargain)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Orders")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                           .Icon.Path(Icons.Icons.Entity.Order)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Order)
                                                           .FilterToParent());
    }
}
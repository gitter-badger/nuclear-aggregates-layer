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
        public static readonly CardMetadata Account =
            CardMetadata.For<Account>()
            .Icon.Path(Icons.Icons.Entity.Account)
                        .CommonCardToolbar()
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                          UIElementMetadata.Config
                                                           .Name.Static("AccountDetails")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelAccountDetails)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.AccountDetail)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Locks")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelLocks)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Lock)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Limits")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Limit)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Orders")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                           .Icon.Path("en_ico_16_Order.gif")
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Order)
                                                           .FilterToParent());
    }
}
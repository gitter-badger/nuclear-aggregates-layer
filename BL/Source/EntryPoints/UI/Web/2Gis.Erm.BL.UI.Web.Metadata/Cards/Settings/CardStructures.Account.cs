using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Account =
            CardMetadata.For<Account>()
                        .MainAttribute(x => x.Id)
                        .ConfigCommonCardToolbar()
                        .ConfigRelatedItems(UiElementMetadata.Config
                                                             .Name.Static("AccountDetails")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelAccountDetails)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.AccountDetail),
                                            UiElementMetadata.Config
                                                             .Name.Static("Locks")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLocks)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Lock),
                                            UiElementMetadata.Config
                                                             .Name.Static("Limits")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Limit),
                                            UiElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .Icon.Path("en_ico_16_Order.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Order));
    }
}
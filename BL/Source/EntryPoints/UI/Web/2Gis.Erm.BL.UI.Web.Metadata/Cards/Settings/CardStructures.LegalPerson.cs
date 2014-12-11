using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata LegalPerson =
            CardMetadata.For<LegalPerson>()
                        .MainAttribute<LegalPerson, ILegalPersonViewModel>(x => x.LegalName)
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Account")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelAccounts)
                                                             .Icon.Path("en_ico_16_Account.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Account)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Limits")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelLimits)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Limit)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Bargains")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelBargains)
                                                             .Icon.Path("en_ico_16_Bargain.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Bargain)
                                                             .FilterToParent(),
                                            UiElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .Icon.Path("en_ico_16_Order.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Order)
                                                             .FilterToParent());
    }
}
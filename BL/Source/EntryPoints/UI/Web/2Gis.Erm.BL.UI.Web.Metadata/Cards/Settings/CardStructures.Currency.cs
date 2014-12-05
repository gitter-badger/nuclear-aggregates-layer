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
        public static readonly CardMetadata Currency =
            CardMetadata.For<Currency>()
                        .MainAttribute<Currency, ICurrencyViewModel>(x => x.Name)
                        .ConfigCommonCardToolbar()
                        .ConfigRelatedItems(
                                    UiElementMetadata.Config.ContentTab("en_ico_16_Currency.gif"),
                                    UiElementMetadata.Config
                                                     .Name.Static("CurrencyRate")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelCurrencyRate)
                                                     .LockOnNew()
                                                     .DisableOn<ICurrencyViewModel>(x => x.IsBase)
                                                     .Handler.ShowGridByConvention(EntityName.CurrencyRate)
                                                     .FilterToParent(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Country")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelCountry)
                                                     .Icon.Path("en_ico_16_Country.gif")
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Country)
                                                     .FilterToParent());
    }
}
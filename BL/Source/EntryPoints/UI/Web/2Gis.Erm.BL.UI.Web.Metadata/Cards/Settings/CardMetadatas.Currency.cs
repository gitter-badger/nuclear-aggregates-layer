using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Currency =
            CardMetadata.For<Currency>()
                        .Icon.Path(Icons.Icons.Entity.Currency)
                        .CommonCardToolbar()
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(Icons.Icons.Entity.CurrencySmall),
                                          UIElementMetadata.Config
                                                           .Name.Static("CurrencyRate")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelCurrencyRate)
                                                           .LockOnNew()
                                                           .DisableOn<ICurrencyViewModel>(x => x.IsBase)
                                                           .Handler.ShowGridByConvention(EntityName.CurrencyRate)
                                                           .FilterToParent(),
                                          UIElementMetadata.Config
                                                           .Name.Static("Country")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelCountry)
                                                           .Icon.Path(Icons.Icons.Entity.CountrySmall)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.Country)
                                                           .FilterToParent());
    }
}
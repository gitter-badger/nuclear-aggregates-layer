using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Currency =
            CardMetadata.Config
                        .For<Currency>()
                        .MainAttribute<Currency, ICurrencyViewModel>(x => x.Name)
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigCommonCardToolbarButtons<Currency>())
                        .RelatedItems
                            .Name("Information")
                            .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                            .Attach(UiElementMetadata.Config.ContentTab(),
                                    UiElementMetadata.Config
                                                     .Name.Static("CurrencyRate")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelCurrencyRate)
                                                     .LockOnNew()
                                                     .DisableOn<ICurrencyViewModel>(x => x.IsBase)
                                                     .Handler.ShowGridByConvention(EntityName.CurrencyRate),
                                    UiElementMetadata.Config
                                                     .Name.Static("Country")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelCountry)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Country));
    }
}
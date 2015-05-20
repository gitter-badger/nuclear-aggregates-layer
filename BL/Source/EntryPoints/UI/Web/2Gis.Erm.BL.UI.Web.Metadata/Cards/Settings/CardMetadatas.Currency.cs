using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Currency =
            CardMetadata.For<Currency>()
                        .WithEntityIcon()
                        .CommonCardToolbar()
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(Icons.Icons.Entity.Small(EntityType.Instance.Currency())),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.CurrencyRate(), () => ErmConfigLocalization.CrdRelCurrencyRate)
                                                      .DisableOn<IBaseCurrencyAspect>(x => x.IsBase),
                                          RelatedItems.RelatedItem.EntityGrid(EntityType.Instance.Country(), Icons.Icons.Entity.Small(EntityType.Instance.Country()), () => ErmConfigLocalization.CrdRelCountry));
    }
}
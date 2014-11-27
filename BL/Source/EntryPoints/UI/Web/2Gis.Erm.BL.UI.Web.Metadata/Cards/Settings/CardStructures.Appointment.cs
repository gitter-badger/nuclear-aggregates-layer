using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        // TODO {y.baranihin, 26.11.2014}: Заполнить MainAttribute
        public static readonly CardMetadata Appointment =
            CardMetadata.For<Appointment>()
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigActivityCardToolbarButtons<Appointment>());
    }
}
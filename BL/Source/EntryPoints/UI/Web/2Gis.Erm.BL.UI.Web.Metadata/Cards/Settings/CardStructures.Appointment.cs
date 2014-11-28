using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Appointment =
            CardMetadata.Config
                        .For<Appointment>()
                        .MainAttribute<Appointment, IAppointmentViewModel>(x => x.Header)
                        .Actions
                            .Attach(UiElementMetadataHelper.ConfigActivityCardToolbarButtons<Appointment>());
    }
}
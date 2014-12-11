using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata UserProfile =
            CardMetadata.For<UserProfile>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(UiElementMetadata.Config.CreateAction<UserProfile>(),
                                UiElementMetadata.Config.UpdateAction<UserProfile>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.SaveAndCloseAction<UserProfile>(),
                                UiElementMetadata.Config.CloseAction());
    }
}
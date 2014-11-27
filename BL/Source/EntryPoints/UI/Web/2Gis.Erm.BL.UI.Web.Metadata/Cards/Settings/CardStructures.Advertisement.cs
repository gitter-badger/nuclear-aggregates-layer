using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Advertisement =
            CardMetadata.For<Advertisement>()
                        .MainAttribute<Advertisement, IAdvertisementViewModel>(x => x.Name)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Advertisement>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Advertisement>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Advertisement>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Preview")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlPreviewAdvertisement)
                                                     .ControlType(ControlType.TextImageButton)
                                                     .LockOnNew()
                                                     .Handler.Name("scope.Preview")
                                                     .Icon.Path("PreviewAd.png"),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction());
    }
}
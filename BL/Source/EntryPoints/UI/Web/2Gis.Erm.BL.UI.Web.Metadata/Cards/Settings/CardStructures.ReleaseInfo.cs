using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata ReleaseInfo =
            CardMetadata.For<ReleaseInfo>()
                        .MainAttribute(x => x.Id)
                        .Actions.Attach(
                            UiElementMetadata.Config.RefreshAction<ReleaseInfo>(),
                            UiElementMetadata.Config.AdditionalActions(UiElementMetadata.Config
                                                                                        .Name.Static("DownloadResults")
                                                                                        .Title.Resource(() => ErmConfigLocalization.ControlDownloadResults)
                                                                                        .ControlType(ControlType.TextButton)
                                                                                        .Handler.Name("scope.DownloadResults")
                                                                                        .LockOnNew()),
                            UiElementMetadata.Config.SplitterAction(),
                            UiElementMetadata.Config.CloseAction());
    }
}
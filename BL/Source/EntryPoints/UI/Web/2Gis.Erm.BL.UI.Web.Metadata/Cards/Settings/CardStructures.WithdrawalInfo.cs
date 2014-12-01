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
        public static readonly CardMetadata WithdrawalInfo =
            CardMetadata.For<WithdrawalInfo>()
                        .MainAttribute(x => x.Id)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<WithdrawalInfo>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<WithdrawalInfo>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<WithdrawalInfo>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.AdditionalActions(UiElementMetadata.Config
                                                                                                .Name.Static("DownloadResults")
                                                                                                .Title.Resource(() => ErmConfigLocalization.ControlDownloadResults)
                                                                                                .LockOnNew()
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .Handler.Name("scope.DownloadResults")),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction());
    }
}
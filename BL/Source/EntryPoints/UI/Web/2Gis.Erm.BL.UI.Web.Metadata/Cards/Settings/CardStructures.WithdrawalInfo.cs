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
                        .Attach(UIElementMetadata.Config.CreateAction<WithdrawalInfo>(),
                                UIElementMetadata.Config.UpdateAction<WithdrawalInfo>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<WithdrawalInfo>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<WithdrawalInfo>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<WithdrawalInfo>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.AdditionalActions(UIElementMetadata.Config
                                                                                            .Name.Static("DownloadResults")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlDownloadResults)
                                                                                            .LockOnNew()
                                                                                            .LockOnInactive()
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .Handler.Name("scope.DownloadResults")),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction());
    }
}
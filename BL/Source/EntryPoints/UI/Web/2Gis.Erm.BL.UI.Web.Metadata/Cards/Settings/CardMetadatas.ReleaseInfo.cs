using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata ReleaseInfo =
            CardMetadata.For<ReleaseInfo>()
                        .MainAttribute(x => x.Id)
                        .Actions.Attach(
                            UIElementMetadata.Config.RefreshAction<ReleaseInfo>(),
                            UIElementMetadata.Config.AdditionalActions(UIElementMetadata.Config
                                                                                        .Name.Static("DownloadResults")
                                                                                        .Title.Resource(() => ErmConfigLocalization.ControlDownloadResults)
                                                                                        .ControlType(ControlType.TextButton)
                                                                                        .Handler.Name("scope.DownloadResults")
                                                                                        .LockOnNew()),
                            UIElementMetadata.Config.SplitterAction(),
                            UIElementMetadata.Config.CloseAction());
    }
}
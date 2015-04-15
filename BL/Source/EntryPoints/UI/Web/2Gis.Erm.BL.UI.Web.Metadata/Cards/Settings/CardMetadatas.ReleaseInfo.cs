using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata ReleaseInfo =
            CardMetadata.For<ReleaseInfo>()
                        .WithDefaultIcon()
                        .Actions.Attach(ToolbarElements.Refresh<ReleaseInfo>(),
                                        ToolbarElements.Additional(ToolbarElements.DownloadResult()),
                                        ToolbarElements.Splitter(),
                                        ToolbarElements.Close());
    }
}
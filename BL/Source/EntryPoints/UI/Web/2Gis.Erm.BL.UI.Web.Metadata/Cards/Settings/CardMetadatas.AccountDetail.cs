using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AccountDetail =
            CardMetadata.For<AccountDetail>()
                        .WithDefaultIcon()
                        .Actions.Attach(ToolbarElements.Create<AccountDetail>(),
                                        ToolbarElements.Update<AccountDetail>(),
                                        ToolbarElements.CreateAndClose<AccountDetail>(),
                                        ToolbarElements.UpdateAndClose<AccountDetail>(),
                                        ToolbarElements.Refresh<AccountDetail>(),
                                        ToolbarElements.Close());
    }
}
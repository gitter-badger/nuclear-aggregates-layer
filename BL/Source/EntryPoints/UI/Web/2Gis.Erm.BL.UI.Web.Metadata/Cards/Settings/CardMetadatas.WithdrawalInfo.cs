using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata WithdrawalInfo =
            CardMetadata.For<WithdrawalInfo>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(ToolbarElements.Create<WithdrawalInfo>(),
                                ToolbarElements.Update<WithdrawalInfo>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<WithdrawalInfo>(),
                                ToolbarElements.UpdateAndClose<WithdrawalInfo>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<WithdrawalInfo>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.WithdrawalInfos.Download()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close());
    }
}
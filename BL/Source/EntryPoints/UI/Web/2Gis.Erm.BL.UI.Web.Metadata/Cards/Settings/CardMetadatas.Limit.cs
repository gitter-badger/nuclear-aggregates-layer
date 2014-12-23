using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Limit =
            CardMetadata.For<Limit>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(ToolbarElements.Create<Limit>(),
                                ToolbarElements.Update<Limit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Limit>(),
                                ToolbarElements.UpdateAndClose<Limit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Limit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.Limits.Approve(),
                                                           ToolbarElements.Limits.Reject(),
                                                           ToolbarElements.Limits.Open(),
                                                           ToolbarElements.Limits.Recalculate()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close());
    }
}
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata UserProfile =
            CardMetadata.For<UserProfile>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(ToolbarElements.Create<UserProfile>(),
                                ToolbarElements.Update<UserProfile>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<UserProfile>(),
                                ToolbarElements.UpdateAndClose<UserProfile>(),
                                ToolbarElements.Close());
    }
}
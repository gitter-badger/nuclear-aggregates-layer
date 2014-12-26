using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Platform =
            CardMetadata.For<Platform.Model.Entities.Erm.Platform>()
                        .WithEntityIcon()
                        .CommonCardToolbar();
    }
}
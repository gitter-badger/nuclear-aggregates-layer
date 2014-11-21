using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata OrderPosition =
            CardMetadata.For<OrderPosition>()
                        .Title.Resource(() => ErmConfigLocalization.EnOrderPositions)
                        .MainAttribute(x => x.Id);
    }
}

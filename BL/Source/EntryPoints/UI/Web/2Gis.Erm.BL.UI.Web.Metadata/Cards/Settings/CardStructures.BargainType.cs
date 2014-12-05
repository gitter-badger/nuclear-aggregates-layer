using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata BargainType =
            CardMetadata.For<BargainType>()
                        .MainAttribute<BargainType, IBargainTypeViewModel>(x => x.Name)
                        .ConfigCommonCardToolbar();
    }
}
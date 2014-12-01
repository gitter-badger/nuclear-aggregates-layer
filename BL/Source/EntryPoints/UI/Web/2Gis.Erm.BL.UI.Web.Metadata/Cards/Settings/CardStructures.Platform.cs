using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Platform =
            CardMetadata.For<Platform.Model.Entities.Erm.Platform>()
                        .MainAttribute<Platform.Model.Entities.Erm.Platform, IPlatformViewModel>(x => x.Name)
                        .ConfigCommonCardToolbar();
    }
}
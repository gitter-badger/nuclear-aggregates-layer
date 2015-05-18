using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Advertisement =
            CardMetadata.For<Advertisement>()
                        .Icon.Path(Icons.Icons.Entity.Small(EntityType.Instance.Advertisement()))
                        .InfoOn<ISelectableToWhiteListAspect>(x => x.IsSelectedToWhiteList,
                                                                             StringResourceDescriptor.Create(() => BLResources.AdvertisementIsSelectedToWhiteList));
    }
}
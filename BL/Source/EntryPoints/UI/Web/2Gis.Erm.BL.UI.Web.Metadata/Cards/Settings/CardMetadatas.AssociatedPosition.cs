using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities.Aggregations;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AssociatedPosition =
            CardMetadata.For<AssociatedPosition>()
                        .InfoOn<INewableAndPublishablePriceAspects>(x => x.PriceIsPublished && x.IsNew,
                                                                    StringResourceDescriptor.Create(() =>
                                                                                                    BLResources
                                                                                                        .CantAddAssociatedPositionToGroupWhenPriceIsPublished))
                        .InfoOn<INewableAndPublishablePriceAspects>(x => x.PriceIsPublished && !x.IsNew,
                                                                    StringResourceDescriptor.Create(() =>
                                                                                                    BLResources
                                                                                                        .CantEditAssociatedPositionInGroupWhenPriceIsPublished))
                        .WithDefaultIcon()
                        .CommonCardToolbar();
    }
}
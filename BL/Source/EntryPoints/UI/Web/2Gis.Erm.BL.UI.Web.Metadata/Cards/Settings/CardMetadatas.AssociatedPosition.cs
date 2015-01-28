using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AssociatedPosition =
            CardMetadata.For<AssociatedPosition>()
                        .InfoOn(StringResourceDescriptor.Create(() => BLResources.CantAddAssociatedPositionToGroupWhenPriceIsPublished))
                            .Func<INewableAspect>(x => x.IsNew)
                            .Func<IPublishablePriceAspect>(x => x.PriceIsPublished)
                            .Combine(ExpressionsCombination.And)
                        .InfoOn(StringResourceDescriptor.Create(() => BLResources.CantEditAssociatedPositionInGroupWhenPriceIsPublished))
                            .Func<INewableAspect>(x => !x.IsNew)
                            .Func<IPublishablePriceAspect>(x => x.PriceIsPublished)
                            .Combine(ExpressionsCombination.And)
                        .WithDefaultIcon()
                        .CommonCardToolbar();
    }
}
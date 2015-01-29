﻿using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AssociatedPosition =
            CardMetadata.For<AssociatedPosition>()
                        .InfoOn(Condition.On<INewableAspect>(x => x.IsNew)
                                         .And(Condition.On<IPublishablePriceAspect>(x => x.PriceIsPublished))
                                         .ToSequence(),
                                StringResourceDescriptor.Create(() => BLResources.CantAddAssociatedPositionToGroupWhenPriceIsPublished))
                        .InfoOn(StringResourceDescriptor.Create(() => BLResources.CantEditAssociatedPositionInGroupWhenPriceIsPublished))
                        .Func<INewableAspect>(x => !x.IsNew)
                        .Func<IPublishablePriceAspect>(x => x.PriceIsPublished)
                        .Combine(LogicalOperation.And)
                        .WithDefaultIcon()
                        .CommonCardToolbar();
    }
}